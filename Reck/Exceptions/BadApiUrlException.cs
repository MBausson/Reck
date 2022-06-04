namespace Reck.Exceptions;

public class BadApiUrlException : Exception
{
    public string ApiUrl { get; private set; } = String.Empty;

    internal BadApiUrlException(string url, string msg) : base($"The specified url ({url}) is incorrect. {msg}")
    {
        ApiUrl = url;
    }
}
