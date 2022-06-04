namespace Reck.Enums;

public class ApiRequestContext
{
    internal Dictionary<string, object> Parameters { get; private set; } = new Dictionary<string, object>();
    private HttpStructure _structure;

    internal ApiRequestContext(HttpStructure str)
    {
        _structure = str;

        int qmi = str.Endpoint.IndexOf('?');
        string paramPart = str.Endpoint.Substring(qmi + 1);

        if (qmi == -1){
            return;
        }

        string[] parameters = paramPart.Split('&');

        foreach (var p in parameters){
            int ei = p.IndexOf('=') + 1;

            if (ei == -1){
                break;
            }

            string name = p.Substring(0, ei - 1).ToLower();
            string value = p.Substring(ei);

            if (Parameters.ContainsKey(name)){
                Parameters[name] = value;
            }
            else{
                Parameters.Add(name, value);
            }
        }
    }

    public object? GetParameter(string name)
    {
        if (ContainsParameter(name)){
            return Parameters[name];
        }

        return null;
    }

    public string GetData()
    {
        return _structure.Body;
    }

    public bool ContainsParameter(string name)
    {
        return Parameters.ContainsKey(name.ToLower());
    }

    public override string ToString()
    {
        return
            $"Reck.Http.Api.ApiRequestContext[{_structure.OperationMethod} | {_structure.Endpoint} | {Parameters.Count} parameter(s) specified]";
    }
}
