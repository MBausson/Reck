using System.Net;
using System;
using System.Text.Encodings.Web;
using System.Web;

namespace Reck.Enums;

public class HttpStructure
{
    public HttpOperationMethod OperationMethod { get; private set; }
    public string Endpoint { get; private set; }
    public string BaseEndPoint { get; private set; } //  Represent the asked url without any parameters
    public string Version { get; private set; }

    public List<HttpHeader> Headers { get; private set; } = new List<HttpHeader>();
    //public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
    public string Body { get; set; }

    public HttpStructure(string content)
    {
        //  Parses the HTTP request into its components
        //  CRLF line breaks
        List<string> lines = content.Split("\r\n").ToList();

        bool processingHeaders = true;

        string[] firstLine = lines[0].Split(" ");

        OperationMethod = getMethod(firstLine[0]);
        Endpoint = HttpUtility.UrlDecode(firstLine[1]);
        Version = firstLine[2];

        //  Getting the absolute endpoint (without the parameter)

        string[] ep = Endpoint.Split('?');

        if (ep.Length != 1){
            BaseEndPoint = ep[0];
        }
        else{
            BaseEndPoint = Endpoint;
        }

        lines.RemoveAt(0); //  We remove the first line of the request, we already processed it

        foreach (var line in lines){
            if (line.Length == 0){
                processingHeaders = false;
                continue;
            }

            if (processingHeaders){
                var h = ProcessHeaderLine(line);
                Headers.Add(h);
                continue;
            }

            Body += $"{line}\r\n";
        }
        //  Remove trailing null characters
        //Body = Body.Trim('\0');   //  Doesn't work ????????
        Body = Body.Replace("\0", string.Empty);
    }

    private HttpHeader ProcessHeaderLine(string line)
    {
        string headerName = "";
        string headerValue = "";
        bool valueProcess = false;

        for (int j = 0; j < line.Length; j++){
            if (line[j] == ':' && !valueProcess){
                valueProcess = true;
                continue;
            }

            if (valueProcess){
                headerValue += line[j];
            }
            else{
                headerName += line[j];
            }

        }

        headerName = headerName.Trim();
        headerValue = headerValue.Trim();

        return new HttpHeader(headerName, headerValue);
    }

    private HttpOperationMethod getMethod(string m)
    {
        m = m.ToLower();
        int maxMethods = Enum.GetNames(typeof(HttpOperationMethod)).Length;

        for (int i = 0; i < maxMethods; i++){
            if (((HttpOperationMethod)i).ToString().ToLower() == m){
                return (HttpOperationMethod)i;
            }
        }

        return HttpOperationMethod.Unknown;
    }

    public override string ToString()
    {
        return $"[{OperationMethod.ToString()}] {Endpoint} => {(Body.Length <= 2 ? "No body" : Body)}";
    }
}