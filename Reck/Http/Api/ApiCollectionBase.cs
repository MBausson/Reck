using System.Reflection;
using NLog;
using Reck.Attributes;
using Reck.Http.Api;

namespace Reck.Enums.Api;

/// <summary>
/// This class collects API endpoints
/// </summary>
public class ApiCollectionBase
{
    internal List<ApiEndPoint> _endPoints { get; private set; } = new List<ApiEndPoint>();

    internal List<ApiCollectionEvent> _endpointReachedHandlers { get; private set; } = new List<ApiCollectionEvent>();

    internal void AddEndpoint(ApiEndPoint endPoint)
    {
        if (_endPoints.Where(e => e.EndPointPath == endPoint.EndPointPath).ToList().Count != 0){
            HttpServer.logger.Fatal($"Endpoint ({endPoint}) is already attached.");
            throw new Exception($"Endpoint ({endPoint}) is already attached.");
        }

        _endPoints.Add(endPoint);
    }

    internal void AddCollectionEvent(ApiCollectionEvent e)
    {
        switch (e.Type){
            case CollectionEventType.OnEndpointReached:
                _endpointReachedHandlers.Add(e);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    /*
     * 1XX
     */

    protected static ApiResponse Continue()
    {
        return new ApiResponse(HttpStatusCode.Continue);
    }

    /*
     * 2XX
     */

    protected static ApiResponse Ok(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.OK);
        }

        return new ApiResponse(HttpStatusCode.OK, obj);
    }

    protected static ApiResponse Created(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.Created);
        }

        return new ApiResponse(HttpStatusCode.Created, obj);
    }

    protected static ApiResponse Information(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.NonAuthoritativeInformation);
        }

        return new ApiResponse(HttpStatusCode.NonAuthoritativeInformation, obj);
    }

    protected static ApiResponse NoContent()
    {
        return new ApiResponse(HttpStatusCode.NoContent);
    }

    protected static ApiResponse ResetContent(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.ResetContent);
        }

        return new ApiResponse(HttpStatusCode.ResetContent, obj);
    }

    protected static ApiResponse PartialContent(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.PartialContent);
        }

        return new ApiResponse(HttpStatusCode.PartialContent, obj);
    }

    /*
     * 3XX
     */

    protected static ApiResponse MovedPermanently(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.MovedPermanently);
        }

        return new ApiResponse(HttpStatusCode.MovedPermanently, obj);
    }

    protected static ApiResponse Found(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.Found);
        }

        return new ApiResponse(HttpStatusCode.Found, obj);
    }

    protected static ApiResponse NotModified(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.NotModified);
        }

        return new ApiResponse(HttpStatusCode.NotModified, obj);
    }

    protected static ApiResponse TemporaryRedirect(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.TemporaryRedirect);
        }

        return new ApiResponse(HttpStatusCode.TemporaryRedirect, obj);
    }

    /*
     * 4XX
     */

    protected static ApiResponse BadRequest(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.BadRequest);
        }

        return new ApiResponse(HttpStatusCode.BadRequest, obj);
    }

    protected static ApiResponse Unauthorized(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.Unauthorized);
        }

        return new ApiResponse(HttpStatusCode.Unauthorized, obj);
    }

    protected static ApiResponse Forbidden(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.Forbidden);
        }

        return new ApiResponse(HttpStatusCode.Forbidden, obj);
    }

    protected static ApiResponse NotFound(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.NotFound);
        }

        return new ApiResponse(HttpStatusCode.NotFound, obj);
    }

    protected static ApiResponse MethodNotAllowed(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.MethodNotAllowed);
        }

        return new ApiResponse(HttpStatusCode.MethodNotAllowed, obj);
    }

    protected static ApiResponse NotAcceptable(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.NotAcceptable);
        }

        return new ApiResponse(HttpStatusCode.NotAcceptable, obj);
    }

    protected static ApiResponse RequestTimeout(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.RequestTimeout);
        }

        return new ApiResponse(HttpStatusCode.RequestTimeout, obj);
    }

    protected static ApiResponse Conflict(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.Conflict);
        }

        return new ApiResponse(HttpStatusCode.Conflict, obj);
    }

    protected static ApiResponse Gone(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.Gone);
        }

        return new ApiResponse(HttpStatusCode.Gone, obj);
    }

    protected static ApiResponse PreconditionFailed(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.PreconditionFailed);
        }

        return new ApiResponse(HttpStatusCode.PreconditionFailed, obj);
    }

    protected static ApiResponse UnsupportedMediaType(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.UnsupportedMediaType);
        }

        return new ApiResponse(HttpStatusCode.UnsupportedMediaType, obj);
    }

    /*
     * 5XX
     */

    protected static ApiResponse InternalServerError(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.InternalServerError);
        }

        return new ApiResponse(HttpStatusCode.InternalServerError, obj);
    }

    protected static ApiResponse NotImplemented(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.NotImplemented);
        }

        return new ApiResponse(HttpStatusCode.NotImplemented, obj);
    }

    protected static ApiResponse ServiceUnavailable(object obj = null)
    {
        if (obj is null){
            return new ApiResponse(HttpStatusCode.ServiceUnavailable);
        }

        return new ApiResponse(HttpStatusCode.ServiceUnavailable, obj);
    }
}
