using Reck.Enums.Api;

namespace Reck.Exceptions;

public class NoCollectionAttribute : Exception
{
    public ApiCollectionBase Collection { get; private set; }

    internal NoCollectionAttribute(ApiCollectionBase coll) : base($"No CollectionAttribute on {coll.GetType().Name}")
    {
        Collection = coll;
    }
}