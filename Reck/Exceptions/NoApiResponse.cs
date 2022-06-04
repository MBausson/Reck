using System.Reflection;

namespace Reck.Exceptions;

public class NoApiResponse : Exception
{
    public MethodInfo Method { get; private set; }

    internal NoApiResponse(MethodInfo methodInfo) : base(
        $"Endpoint '{methodInfo}' must return an ApiResponse (or Task<ApiResponse>) object.\nGot {methodInfo.ReturnType} instead.")
    {
        Method = methodInfo;
    }
}