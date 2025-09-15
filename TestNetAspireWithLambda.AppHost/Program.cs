#pragma warning disable CA2252

var builder = DistributedApplication.CreateBuilder(args);

// Add Lambda API
var lambdaFunction = builder.AddAWSLambdaFunction<Projects.LambdaCoreWebAPI>("lambda-core-web-api",
    "LambdaCoreWebAPI::LambdaCoreWebAPI.LambdaEntryPoint::FunctionHandlerAsync");
builder.AddAWSAPIGatewayEmulator("APIGatewayEmulator", Aspire.Hosting.AWS.Lambda.APIGatewayType.HttpV2)
    .WithReference(lambdaFunction, Aspire.Hosting.AWS.Lambda.Method.Any, "/{proxy+}");


// Working with Redis Cache for the Sample Web API Project
var redis = builder.AddRedis("redis")
                                             .WithContainerName("redis-container"); // Friendly name for the container
builder.AddProject<Projects.SampleWebAPI>("web-api")
    .WithReference(redis)
    .WithEndpoint(port: 5050, scheme: "http", name: "webapi-http");  // If we want to add additional endpoint to point to the web api endpoint.

builder.Build().Run();