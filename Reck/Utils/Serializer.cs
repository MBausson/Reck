using System.Text.Json;
using System.Xml.Serialization;
using Reck.Enums;

namespace Reck.Utils;

public sealed class Serializer
{
    public static string ToJson(object obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

    public static string ToXML(object obj)
    {
        var serializer = new XmlSerializer(obj.GetType());
        StringWriter sw = new StringWriter();

        serializer.Serialize(sw, obj);
        return sw.ToString();
    }
}