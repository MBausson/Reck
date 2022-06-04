using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Reck.Enums.Cors;
using Reck.Utils;


namespace Reck.Enums.Api;

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string Body { get; set; } = "";
    public List<HttpHeader> AdditionalHeaders { get; set; } = new List<HttpHeader>();
    private List<HttpHeader> Headers { get; set; } = new List<HttpHeader>();

    /// <summary>
    /// Complete ApiResponse constructor for json output
    /// </summary>
    /// <param name="statusCode">Response status code (OK, NOT FOUND, ...)</param>
    /// <param name="body">The data to send</param>
    /// <param name="opt_headers">Additional headers</param>
    /// <remarks>If the body passed is a string, the body would be a JSON dictionary with <c>message: yourBody</c></remarks>
    public ApiResponse(HttpStatusCode statusCode, object body, params HttpHeader[] opt_headers)
    {
        StatusCode = statusCode;

        if (!(body is string)){
            Body = Serializer.ToJson(body);
        }
        else{
            Body = $"{{\"message\": \"{body}\"}}";
        }

        Headers.Add(new HttpHeader("Content-Type", "application/json; charset=utf-8"));

        AddUsualHeaders();
        Headers.AddRange(AdditionalHeaders);
        Headers.AddRange(opt_headers);
    }

    /// <summary>
    /// Complete ApiResponse constructor for raw strings response
    /// </summary>
    /// <param name="code">Response status code (OK, NOT FOUND, ...)</param>
    /// <param name="body">The body as string</param>
    /// <param name="opt_headers">Additional headers</param>
    /// <remarks>The response's body will be the exact string passed (<c>body</c>)</remarks>
    public ApiResponse(HttpStatusCode code, string body, params HttpHeader[] opt_headers)
    {
        StatusCode = code;
        Body = body;

        Headers.Add(new HttpHeader("Content-Type", "text/plain; charset=utf-8"));

        AddUsualHeaders();
        Headers.AddRange(AdditionalHeaders);
        Headers.AddRange(opt_headers);
    }

    /// <summary>
    /// ApiResponse constructor for body-less responses
    /// </summary>
    /// <param name="status">Response status code (OK, NOT FOUND, ...)</param>
    /// <param name="opt_headers">Additional headers</param>
    public ApiResponse(HttpStatusCode status, params HttpHeader[] opt_headers)
    {
        StatusCode = status;

        Headers.Add(new HttpHeader("Content-Type", "text/plain; charset=utf-8"));

        AddUsualHeaders();
        Headers.AddRange(AdditionalHeaders);
        Headers.AddRange(opt_headers);
    }

    public void AddHeaders(params HttpHeader[] headers)
    {
        foreach (var h in headers){
            AddHeader(h);
        }
    }

    public void AddHeader(HttpHeader header)
    {
        var already_defined_header = Headers.Where(h => h.Key == header.Key).FirstOrDefault();

        //  If our response already contains the header, we just modify its value
        if (already_defined_header != null){
            already_defined_header.Value = header.Value;
            return;
        }

        //  Create the header
        Headers.Add(header);
    }

    private void AddUsualHeaders()
    {
        AddHeader(new HttpHeader("Connection", "close"));
        AddHeader(new HttpHeader("Date", DateTime.Now.ToString("R")));
        AddHeader(new HttpHeader("Content-Length", Body.Length));
    }

    internal string BuildHttpResponse()
    {
        string res = String.Empty;

        //  Status line
        res += $"HTTP/1.1 {(int)StatusCode} {StatusCode.ToString().ToUpper()}\r\n";

        //  Headers
        foreach (var header in Headers){
            res += $"{header.Key}: {header.Value}\r\n";
        }

        //  Message
        res += "\r\n";
        res += $"{Body}";

        return res;
    }

    internal void AddCorsHeaders(HttpPolicy policy)
    {
        string value = "";

        //  Origin

        value = policy.AcceptAllOrigins ? "*" : String.Join(',', policy.AcceptedOrigins);

        AddHeader(new HttpHeader("Access-Control-Allow-Origin", value));

        value = "";

        //  Methods

        value = policy.AcceptAllOperationMethods ? "*" : String.Join(',', policy.AcceptedOperationMethods);

        AddHeader(new HttpHeader("Access-Control-Request-Method", value));
        value = "";

        //  Headers

        value = policy.AcceptAllHeaders ? "*" : String.Join(',', policy.AcceptedHeaders);

        AddHeader(new HttpHeader("Access-Control-Request-Headers", value));
    }
}
