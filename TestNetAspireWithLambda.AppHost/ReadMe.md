## Startup Code

```csharp
#pragma warning disable CA2252


var builder = DistributedApplication.CreateBuilder(args);

var lambdaFunction = builder.AddAWSLambdaFunction<Projects.LambdaCoreWebAPI>("lambda-core-web-api", 
    "LambdaCoreWebAPI::LambdaCoreWebAPI.LambdaEntryPoint::FunctionHandlerAsync");

builder.AddAWSAPIGatewayEmulator("APIGatewayEmulator", Aspire.Hosting.AWS.Lambda.APIGatewayType.HttpV2)
    .WithReference(lambdaFunction, Aspire.Hosting.AWS.Lambda.Method.Get, "/{proxy+}");

builder.Build().Run();
```


## Lambda Core Web API - Important Things to consider

1. We will need to update LambdaEntryPoint.cs to inherit from `Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction` instead of `Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction`.
2. This is because our Aspire host is configure to use `APIGatewayType.HttpV2` (Http API v2.0)
```csharp
builder.AddAWSAPIGatewayEmulator("APIGatewayEmulator", Aspire.Hosting.AWS.Lambda.APIGatewayType.HttpV2)
    .WithReference(lambdaFunction, Aspire.Hosting.AWS.Lambda.Method.Get, "/{proxy+}");
```
3. The handler value can also be found on `serverless.template`
4. We are using `/{proxy+}` to handle all kinds of GET Requests. If you want to handle other HTTP methods, you will need to add them as well.

5. When we spin the .Net Aspire, now the AWS Gateway Emulator will be expose on http://localhost:50912/ and it can now handle the request coming to the LambdaCoreWebAPI



## Sample Web API - Import things to consider for Redis
1. In the web api project, we will need to add RedisClient and registered any custom service related to Redis.
```csharp
   // Add Redis Cache Service
   builder.AddRedisClient("redis");
   builder.Services.AddScoped<RedisCacheService>();
```

2. The Web API will need to install `Aspire.StackExchange.Redis` nuget package which is Aspire compatible Nuget package. 

Note: Your Web API project should reference the Aspire.StackExchange.Redis package to use the Redis client and connect via dependency injection.
Your .NET Aspire host project (the orchestration project) should reference the Aspire.Hosting.Redis package to declare and manage the Redis resource.
This setup allows Aspire to wire up the Redis resource, inject the connection string, and enable dashboard tracing and monitoring.

3. The .Net Aspire Host project will need to have nuget package `<PackageReference Include="Aspire.Hosting.Redis" Version="9.4.2" />` and

```csharp
// Working with Redis Cache for the Sample Web API Project
var redis = builder.AddRedis("redis")
                                             .WithContainerName("redis-container"); // Friendly name for the container
builder.AddProject<Projects.SampleWebAPI>("web-api")
    .WithReference(redis)
    .WithHttpsEndpoint(port: 5050, name: "web-api");
```