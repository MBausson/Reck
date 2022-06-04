using System;
using System.Diagnostics;

namespace Reck.Enums.Cors;

public class HttpCorsPolicies
{
    /// <summary>
    /// All registered policies
    /// </summary>
    public List<HttpPolicy> Policies { get; set; } = new List<HttpPolicy>();

    /// <summary>
    /// This policy is the reference one for all endpoints
    /// </summary>
    public HttpPolicy DefaultPolicy { get; private set; } = HttpPolicy.All;

    public HttpCorsPolicies(params HttpPolicy[] policies)
    {
        foreach (var p in policies){
            AddPolicy(p);
        }
    }

    public void AddPolicy(HttpPolicy policy)
    {
        Policies.Add(policy);

        if (policy.AllEndPoints){
            DefaultPolicy = policy;
        }
    }

    public HttpPolicy? GetPolicyForEndpoint(string baseEndPoint)
    {
        if (baseEndPoint.Length == 0){
            return DefaultPolicy;
        }

        return Policies.Where(p => p.EndPoint == baseEndPoint).FirstOrDefault();
    }
}
