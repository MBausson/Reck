#   Reck
___

### */!\ This project was created for learning purpose, it's highly likely that the code in it is bad.*  

## Explainations
The term '**Endpoint**' designate the link to a specific resource.  
In Reck, endpoints are grouped together in '**Collections**'.  
Collection, just like endpoints have a name, and represent a part of the final endpoints' **URL**.  

**_For example:_**  
Let's imagine a collection 'v1' containing an endpoint 'weather'.  
The URL to access the endpoint 'weather' is `/v1/weather`  

Furthermore, collections are embedded in Api objects, which are basically collections of collections.  
Though, they are **_not_** represented in endpoint's URLS (`maybe they should ?`)  

---
## Usage

Let's have a look to this example:
```csharp
public static void Main(string[] args)
{
    HttpServer server = new HttpServer("localhost", 50001);
    
    HttpApi api = new HttpApi();
    api.AddCollection(new GreetingsCollection());
    
    server.AddApi(api);
    server.Start();
}
```

First of all, we define a `HttpServer` object, _localhost and at port `50001`_  
We then define a `HttpApi`, for which we **add a `GreetingsCollection` collection**.  
After that, we add our previously defined API to our server.  
At the end, our server is started and now starts receiving and responding to HTTP requests.  

## Collections

To create a collection, you simply have create a class which inherits from `ApiCollectionBase`, and have a `[Collection(<name>)]` attribute to it.  
Now we need to create an endpoint for our collection.  

Endpoints are defined as methods returning a `ApiResponse` or `Task<ApiResponse>`, having a `HttpRequestContext` parameter, and having the `[EndPoint(<Operation>, <Name>)]` attribute to them.

##  Time-saving features

When returning a response, you can instantiate `ApiResponse` or simply use your collection **default methods** such as `Ok()`, `NotFound()` or even `MethodNotAllowed()`.

Furthermore, Reck provides collection-wide events, such as `OnEndpointReached`, which is called everytime (and before the response) an endpoint is requested.  
To use them, simply create a method with the **attribute** `[CollectionEvent(<eventType>)]`, with either no parameter, an `ApiRequestContext` or/and `HttpStructure`.  

You can ask for URL parameters, by specifying them in your method's parameters, with the corresponding name.  
A URL parameter can also be optional, with the `[OptionalParam]` attribute on it, or by specifying a default value :  

```csharp
[EndPoint(HttpOperationMethod.Get, "isadult")]
public ApiResponse IsAdultMethod(ApiRequestContext ctx, string name = "User", int age)
```

Here, the endpoint `isadult` take two URL parameters: `name` which is optional and has a defaut value of `"User"`, and an integer `age` which is required.  
Reck sends a **400 Bad Request** when required parameters aren't specified.
