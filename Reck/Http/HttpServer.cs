using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using NLog;
using Reck.Attributes;
using Reck.Enums.Api;
using Reck.Enums.Cors;

namespace Reck.Enums;

public class HttpServer
{
    public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    public string HostName { get; private set; }
    public int Port { get; private set; }

    public HttpCorsPolicies CorsPolicies { get; set; } = new HttpCorsPolicies();

    private IPEndPoint _localEndpoint;
    private Socket _localSocket;

    private List<HttpApi> _httpApis { get; set; } = new List<HttpApi>();

    public HttpServer(string hostname, int port)
    {
        IPHostEntry he = Dns.GetHostEntry(hostname);
        IPAddress ia = he.AddressList[1];

        _localEndpoint = new IPEndPoint(ia, port);
        _localSocket = new Socket(_localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _localSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

        HostName = hostname;
        Port = port;

        logger.Info($"------------------");
        logger.Info($"HttpServer address is now '{HostName}' @ {Port}");
    }

    public HttpServer(HttpServerSettings settings) : this(settings.HostName, settings.Port)
    {
        foreach (var policy in settings.CorsPolicies){
            CorsPolicies.AddPolicy(policy);
        }
    }

    public void AddApi(HttpApi api)
    {
        _httpApis.Add(api);
    }

    public void Start()
    {
        //  Bind & listen
        _localSocket.Bind(_localEndpoint);
        _localSocket.Listen();

        //  Logging
        logger.Info($"HttpServer is now listening on '{HostName}' @ {Port}");

            //  Cors log
        logger.Info($"HttpServer has {CorsPolicies.Policies.Count} CORS policies.");
        logger.Info($"\t* => Origins : {CorsPolicies.DefaultPolicy.AcceptAllOrigins}");
        logger.Info($"\t* => Methods : {CorsPolicies.DefaultPolicy.AcceptAllOperationMethods}");
        logger.Info($"\t* => Headers : {CorsPolicies.DefaultPolicy.AcceptAllHeaders}");

        //  Our buffer fot the request -- the limit is 1Ko, we should make it bigger
        byte[] data = new byte[1024];

        while (true){
            //  Blocking here, waiting for a request
            Socket remote = _localSocket.Accept();
            remote.Receive(data);

            //  Here, we have a request and we process it in another thread so we can continue getting other request while processing one

            Task.Run(async () =>
            {
                //  In case the request process fails -> send a 500
                try{
                    await HandleRequest(remote, data);
                }
                catch (Exception e){
                    logger.Error($"Error while processing a request: \n{e.Message}\n{e.StackTrace}");

                    InternalError(remote, e);
                }
                finally{
                    remote.Close();
                }

            });
        }
    }

    /*
     * This methods proceeds in 5 steps:
     *  1 - Endpoint recognition
     *  2 - Check for endpoint not found or bad method
     *  3 - Parameters processing
     *  4 - Check for wrong parameters
     *  5 - If reached, execute the corresponding ApiEndPoint method and send the ApiResponse returned
     *
     *  If any internal error occurs while executing this method, it would have to be handled outside of this method
     *      and send a response 500
     */
    private async Task HandleRequest(Socket sender, byte[] data)
    {
        HttpStructure structure = new HttpStructure(Encoding.ASCII.GetString(data));
        ApiRequestContext ctx = new ApiRequestContext(structure);
        ApiEndPoint reachedEndPoint = null; //  Null until we find the specified end point

        //  Describes the state of the request process, reveals any problem ( 404, 400, ... )
        ResponseProcessType currentState = ResponseProcessType.NOT_FOUND;

        /*
         * 1 - Endpoint recognition
         */

        /*
         * Here, we search for the specified endpoint.
         * We set processType to bad method if the found endpoint does not match specified operation method
         * We set processType to found if we find the correct endpoint
         */
        foreach (var api in _httpApis){
            foreach (var collection in api._collectionBases){
                foreach (var endPoint in collection._endPoints){

                    //  We have to compare base endpoints, without additionals parameters
                    if (endPoint.EndPointPath == structure.BaseEndPoint){
                        reachedEndPoint = endPoint;

                        if (endPoint.EpAttr.OperationMethod != structure.OperationMethod){
                            currentState = ResponseProcessType.BAD_METHOD;
                            break;
                        }

                        //  Trigger the on reached endpoint events
                        foreach (var e in collection._endpointReachedHandlers){
                            var activator = Activator.CreateInstance(e.MethodInfo.DeclaringType);

                            List<object> arguments = new List<object>();

                            if (e.HasContextParameter){
                                arguments.Add(ctx);
                            }
                            if (e.HasStructureParameter){
                                arguments.Add(structure);
                            }

                            e.MethodInfo.Invoke(activator, arguments.ToArray());
                        }

                        currentState = ResponseProcessType.FOUND;
                    }

                }
            }
        }

        /*
         * 2 - Notfound or BadMethod handling
         */

        if (currentState == ResponseProcessType.NOT_FOUND){
            RefuseNotFound(sender, structure.BaseEndPoint);
            logger.Trace($"[404] for '{structure.Endpoint}' ({structure.BaseEndPoint}).");
            return;
        }

        if (currentState == ResponseProcessType.BAD_METHOD){
            RefuseBadMethod(sender, structure.OperationMethod, reachedEndPoint.EpAttr.OperationMethod);
            logger.Trace(
                $"[405] for '{structure.Endpoint}' (Expected {reachedEndPoint.Method.ToString()}, got {structure.OperationMethod.ToString()}).");
            return;
        }

        /*
         * 3 - Request parameters recognition
         */

        List<object> methodArguments = new List<object>();
        methodArguments.Add(ctx);

        //  For each expected parameter, we check if it's specified and if it's optional
        //  If a required parameter is not specified, we send a RefuseBadParameterType
        foreach (var apiParameter in reachedEndPoint.UrlParameters){
            //  This get the url parameter -- null if not found
            var value = ctx.GetParameter(apiParameter.Name);

            //  If the parameter is not specified in the request's url
            if (value is null){
                //  And if the parameter is mandatory
                if (!apiParameter.IsOptional){
                    currentState = ResponseProcessType.BAD_PARAMETERS;
                    break;
                }

                //  If it is optional, we just set add null for the parameter's value
                methodArguments.Add(apiParameter.DefaultValue);
                continue;
            }

            object p;

            try{
                p = Convert.ChangeType(value, apiParameter.Type);
            }
            catch (FormatException){
                RefuseBadParameterType(sender, apiParameter.Name, apiParameter.Type);
                return;
            }
            catch (InvalidCastException){
                throw new Exception($"The specified parameter's type does not implement 'IConvertible'.");
            }

            methodArguments.Add(p);
        }

        /*
         * 4 - Check for bad parameter
         */

        if (currentState == ResponseProcessType.BAD_PARAMETERS){
            RefuseBadParameters(sender, reachedEndPoint.UrlParameters.Count, reachedEndPoint.UrlParameters.Where(p => !p.IsOptional).Count());
            logger.Trace(
                $"[400] for '{structure.Endpoint}'. Either not enough parameters or required ones are unspecified.");
            return;
        }

        /*
         *  5 - Execute and send the corresponding api response
         */

        //  At this point, there's no error, everything is correct

        var instance = Activator.CreateInstance(reachedEndPoint.Method.DeclaringType);

        //  This object will be the developer-defined response for the request.
        ApiResponse response;

        //  If the method is async, we expect Task<ApiResponse> instead of ApiResponse and await the method
        if (reachedEndPoint.IsAsync){
            var asyncResponse = reachedEndPoint.Method.Invoke(instance, methodArguments.ToArray()) as Task<ApiResponse>;
            response = await asyncResponse;
        }
        else{
            response = reachedEndPoint.Method.Invoke(instance, methodArguments.ToArray()) as ApiResponse;
        }

        /*
         * Cors policies
         * We basically add the corresponding headers
         */

        HttpPolicy policy = CorsPolicies.GetPolicyForEndpoint(reachedEndPoint.EndPointPath);

        //  If there's no special policy for the endpoint, choose the default one
        if (policy is null){
            policy = CorsPolicies.DefaultPolicy;
        }

        //  Adds CORS headers based on the current endpoint
        response.AddCorsHeaders(policy);

        sender.Send(Encoding.ASCII.GetBytes(response.BuildHttpResponse()));
        logger.Trace($"[200] for {structure.OperationMethod.ToString().ToUpper()} => {structure.Endpoint}");

        //  The socket is closed outside of this function, we don't have to bother about it here
    }

    /*
     *
     * Default responses for errors
     *
     */

    private static void RefuseNotFound(Socket remote, string specifiedEp)
    {
        remote.Send(Encoding.ASCII.GetBytes(
            new ApiResponse(HttpStatusCode.NotFound, $"Endpoint ({specifiedEp}) not found.").BuildHttpResponse()
        ));
    }

    private static void RefuseBadMethod(Socket remote, HttpOperationMethod specified, HttpOperationMethod expected)
    {
        remote.Send(Encoding.ASCII.GetBytes(
            new ApiResponse(HttpStatusCode.MethodNotAllowed,
                $"Method is incorrect.\r\nExpected '{expected}' but got '{specified}'").BuildHttpResponse()
        ));
    }

    private static void RefuseBadParameters(Socket remote, int nParameters, int nRequiredParameters)
    {
        remote.Send(Encoding.ASCII.GetBytes(
            new ApiResponse(HttpStatusCode.BadRequest,
                    $"Bad parameters : expected {nParameters} parameter(s) ( {nRequiredParameters} of whose are required ).")
                .BuildHttpResponse()
        ));
    }

    private static void RefuseBadParameterType(Socket remote, string paramName, Type expectedType)
    {

        remote.Send(Encoding.ASCII.GetBytes(
            new ApiResponse(HttpStatusCode.BadRequest, $"Cannot get parameter of type '{expectedType}' for <{paramName}>")
                .BuildHttpResponse()
        ));
    }

    private static void InternalError(Socket remote, Exception e)
    {
        remote.Send(Encoding.ASCII.GetBytes(new ApiResponse(HttpStatusCode.InternalServerError,
                $"An error occured while processing your request :\r\n{e.Message}\r\n{e.StackTrace}")
            .BuildHttpResponse()));
    }
}
