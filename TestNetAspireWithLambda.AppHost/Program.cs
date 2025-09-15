#pragma warning disable CA2252

var builder = DistributedApplication.CreateBuilder(args);

// // Add Lambda API
// var lambdaFunction = builder.AddAWSLambdaFunction<Projects.LambdaCoreWebAPI>("lambda-core-web-api",
//     "LambdaCoreWebAPI::LambdaCoreWebAPI.LambdaEntryPoint::FunctionHandlerAsync");
// builder.AddAWSAPIGatewayEmulator("APIGatewayEmulator", Aspire.Hosting.AWS.Lambda.APIGatewayType.HttpV2)
//     .WithReference(lambdaFunction, Aspire.Hosting.AWS.Lambda.Method.Any, "/{proxy+}");


// Working with Redis Cache for the Sample Web API Project
var redis = builder.AddRedis("sanz-redis")
    .WithRedisCommander(); // Redis Cache GUI to view key/value
    
builder.AddProject<Projects.SampleWebAPI>("web-api")
    .WithReference(redis)
    .WithHttpsEndpoint(port: 5050, name: "web-api");

builder.Build().Run();