namespace Reck.Enums.Cors;

public class HttpPolicy
{
    /// <summary>
    /// The endpoint for whose the policy is applied
    /// </summary>
    /// <remarks>An empty endPoint means the policy applies for every endpoints</remarks>
    public string EndPoint { get; set; } = String.Empty;
    public bool AllEndPoints => EndPoint.Length <= 1;

    public List<string> AcceptedOrigins { get; set; } = new List<string>();
    public bool AcceptAllOrigins => AcceptedOrigins.Count == 0;

    public List<HttpOperationMethod> AcceptedOperationMethods { get; set; } = new List<HttpOperationMethod>();
    public bool AcceptAllOperationMethods => AcceptedOperationMethods.Count == 0;

    public List<string> AcceptedHeaders { get; set; } = new List<string>();
    public bool AcceptAllHeaders => AcceptedHeaders.Count == 0;

    public HttpPolicy(string endPoint = "")
    {
        if (endPoint.Length != 0 && endPoint[0] != '/'){
            EndPoint = $"/{endPoint}";
        }
        else{
            EndPoint = endPoint;
        }
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is HttpPolicy)){
            return false;
        }

        HttpPolicy p = (HttpPolicy)obj;

        return EndPoint == p.EndPoint;
    }

    public static readonly HttpPolicy All = new HttpPolicy();
}
