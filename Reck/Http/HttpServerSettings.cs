using System.Text.Json;
using Reck.Enums.Cors;

namespace Reck.Enums;

public struct HttpServerSettings
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public List<HttpPolicy> CorsPolicies { get; set; }

    public HttpServerSettings(string host, int port)
    {
        HostName = host;
        Port = port;
        CorsPolicies = new List<HttpPolicy>();
    }
    public static HttpServerSettings FromJsonFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        HttpServerSettings settings = JsonSerializer.Deserialize<HttpServerSettings>(json);

        if (settings.CorsPolicies is null){
            settings.CorsPolicies = new List<HttpPolicy>();
        }

        return settings;
    }

    public override string ToString()
    {
        return $"Reck.Http.HttpServerSettings[{HostName} | :{Port} | {CorsPolicies.Count} CORS policies]";
    }
}
