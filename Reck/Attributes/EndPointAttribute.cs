using System.Text.RegularExpressions;
using System.Threading.Channels;
using Reck.Enums;
using Reck.Utils;

namespace Reck.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class EndPointAttribute : Attribute
{
    public HttpOperationMethod OperationMethod { get; private set; }
    public string Endpoint { get; private set; }

    internal Regex rgx = new Regex("[^A-Za-z0-9]");


    public EndPointAttribute(HttpOperationMethod operationMethod, string endpoint)
    {
        OperationMethod = operationMethod;
        Endpoint = endpoint;

        Endpoint = UrlFormatter.FormatDirectoryName(endpoint);
    }
}
