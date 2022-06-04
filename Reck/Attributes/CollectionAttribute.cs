using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Reck.Enums.Api;
using Reck.Utils;

namespace Reck.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CollectionAttribute : System.Attribute
{
    public string Name { get; private set; }
    internal List<ApiEndPoint> endPoints = new List<ApiEndPoint>();

    public CollectionAttribute(string name)
    {
        Name = UrlFormatter.FormatBaseUrl(name);
    }
}
