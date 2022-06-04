using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Reck.Enums;
using Reck.Attributes;

namespace Reck.Enums.Api;

internal class ApiEndPoint
{
    public string EndPointPath { get; private set; }
    public MethodInfo Method { get; private set; }
    public CollectionAttribute CollectionAttr { get; private set; }
    public EndPointAttribute EpAttr { get; private set; }
    public bool IsAsync { get; private set; }
    //  todo: Maybe consider using a queue ?
    public List<HttpUrlParameter> UrlParameters { get; private set; } = new List<HttpUrlParameter>();

    internal ApiEndPoint(CollectionAttribute coll, EndPointAttribute ep, MethodInfo meth)
    {
        Method = meth;
        CollectionAttr = coll;
        EpAttr = ep;
        EndPointPath = $"{coll.Name}/{EpAttr.Endpoint}";
        IsAsync = !(Method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) as AsyncStateMachineAttribute == null);
    }

    internal void AddUrlParameter(HttpUrlParameter parameter)
    {
        UrlParameters.Add(parameter);
    }

    public override string ToString()
    {
        return $"Reck.Http.Api.ApiEndPoint[{EndPointPath} | {Method.Name} | {UrlParameters.Count} parameter(s)]";
    }
}
