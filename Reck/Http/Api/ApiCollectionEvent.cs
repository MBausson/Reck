using System.Reflection;
using Reck.Enums;

namespace Reck.Http.Api;

internal class ApiCollectionEvent
{
    public MethodInfo MethodInfo { get; init; }
    public bool HasContextParameter { get; init; }
    public bool HasStructureParameter { get; init; }
    public bool HasParameters => HasContextParameter || HasStructureParameter;
    public CollectionEventType Type { get; init; }

    internal ApiCollectionEvent(MethodInfo methodInfo, CollectionEventType type, bool hasStructureParameter, bool hasContextParameter)
    {
        MethodInfo = methodInfo;
        Type = type;
        HasContextParameter = hasStructureParameter;
        HasContextParameter = hasContextParameter;
    }

    public override string ToString()
    {
        return $"{MethodInfo.GetBaseDefinition()}[{Type}]";
    }
}
