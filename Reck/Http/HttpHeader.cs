namespace Reck.Enums;

public class HttpHeader
{
    public string Key { get; set; }
    public object Value { get; set; }

    public HttpHeader(string key, object value)
    {
        Key = key;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Key}: {Value}";
    }

    /// <summary>
    /// The HttpHeader's equal operation relies on both HttpHeader's 'Key' field.
    /// </summary>
    /// <returns>True if the objects have the same key.</returns>
    public override bool Equals(object? obj)
    {
        HttpHeader header = obj as HttpHeader;

        if (header == null){
            return false;
        }

        return Key == header.Key;
    }
}
