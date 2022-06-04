using System.Reflection;
using System.Runtime.CompilerServices;
using Reck.Exceptions;
using Reck.Attributes;
using Reck.Attributes.CollectionEvents;
using Reck.Http.Api;

namespace Reck.Enums.Api;

public sealed class HttpApi
{
    internal List<ApiCollectionBase> _collectionBases { get; set; } = new List<ApiCollectionBase>();
    internal static Type asyncType = typeof(AsyncStateMachineAttribute);
    internal static Type[] AcceptedParametersTypes = new[]
    {
        typeof(ApiRequestContext),
        typeof(HttpStructure)
    };

    public void AddCollection(ApiCollectionBase coll)
    {
        var collAttr = coll.GetType().GetCustomAttribute(typeof(CollectionAttribute), false) as CollectionAttribute;

        if (collAttr == null){
            HttpServer.logger.Error($"No CollectionAttribute on {coll.GetType().Name}");
            throw new NoCollectionAttribute(coll);
        }

        //  Adds the endpoints to the collection ( detects EndPoint flagged methods ) or adds events to the collection internal lists
        foreach (var m in coll.GetType().GetMethods()){
            if (!m.IsPublic && !m.IsStatic) continue; //  Ignores statics and non-public methods

            //  First, check for endpoint method
            var epAttr = m.GetCustomAttribute(typeof(EndPointAttribute), false) as EndPointAttribute;

            if (epAttr != null){
                ApiEndPoint ep = RegisterEndPointMethod(m, collAttr, epAttr);

                coll.AddEndpoint(ep);
                HttpServer.logger.Info($"Registered new endpoint => {ep}");
                continue;
            }

            //  Otherwise, check for collection events
            var eventAttr = m.GetCustomAttributes(typeof(CollectionEventAttribute), false) as CollectionEventAttribute[];

            foreach (var attribute in eventAttr){
                var e = RegisterCollectionEvent(m, attribute);
                coll.AddCollectionEvent(e);
                HttpServer.logger.Info($"Registered new collection event on collection <{coll}> => `{e}`");
            }

        }

        _collectionBases.Add(coll);
    }

    private ApiEndPoint RegisterEndPointMethod(MethodInfo m, CollectionAttribute collAttr, EndPointAttribute epAttr)
    {
        //  If the method doesn't return ApiResponse
        var asyncAttrib = (AsyncStateMachineAttribute)m.GetCustomAttribute(asyncType);

        if ((asyncAttrib != null && m.ReturnType != typeof(Task<ApiResponse>)) || m.ReturnType != typeof(ApiResponse)){
            HttpServer.logger.Error(
                $"Endpoint '{m}' must return an ApiResponse (or Task<ApiResponse>) object.\nGot {m} instead.");
            throw new NoApiResponse(m);
        }

        //  If the method doesn't have a context parameter
        var parameters = m.GetParameters().ToList();

        if (parameters.Count == 0 || parameters[0].ParameterType != typeof(ApiRequestContext)){
            HttpServer.logger.Error($"Endpoint ({m}) method must contain an ApiRequestContext parameter.");
            throw new NoContextException(m);
        }

        //  We are gonna process the method's parameters, we need to remove the context parameter which is irrelevant now
        parameters.RemoveAt(0);

        ApiEndPoint ep = new ApiEndPoint(collAttr, epAttr, m);

        foreach (var param in parameters){
            ep.AddUrlParameter(new HttpUrlParameter(param));
        }

        return ep;
    }

    private ApiCollectionEvent RegisterCollectionEvent(MethodInfo methodInfo, CollectionEventAttribute eventAttribute)
    {
        var methodParameters = methodInfo.GetParameters();

        bool hasStructureParameter =
            methodParameters.Select(p => p.ParameterType).Count(p => p == typeof(HttpStructure)) != 0;

        bool hasContextParameter =
            methodParameters.Select(p => p.ParameterType).Count(p => p == typeof(ApiRequestContext)) != 0;

        //  A collection event can either take no argument, a ApiRequestContext or HttpStructure parameters
        if (methodParameters.Length > 0 && !(hasContextParameter || hasStructureParameter)){
            //  Here, the method does not contains the accepted parameters
            throw new Exception($"A CollectionEvent method must either take no argument, HttpStructure and/or ApiRequestContext");
        }

        return new ApiCollectionEvent(methodInfo, eventAttribute.EventType, hasStructureParameter, hasContextParameter);
    }
}
