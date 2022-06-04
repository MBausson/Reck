using System.Reflection;

namespace Reck.Exceptions;

public class NoContextException : Exception
{
    public MethodInfo Method { get; private set; }

    internal NoContextException(MethodInfo methodInfo) : base($"No context parameter for '{methodInfo.Name}'")
    {
        Method = methodInfo;
    }
}