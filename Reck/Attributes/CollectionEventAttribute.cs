using Reck.Enums;

namespace Reck.Attributes.CollectionEvents;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Delegate, AllowMultiple = false)]
public class CollectionEventAttribute : System.Attribute
{
    internal CollectionEventType EventType { get; init; }

    public CollectionEventAttribute(CollectionEventType type)
    {
        EventType = type;
    }
}
