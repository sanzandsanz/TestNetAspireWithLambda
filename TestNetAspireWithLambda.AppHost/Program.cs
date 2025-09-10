#pragma warning disable CA2252


var builder = DistributedApplication.CreateBuilder(args);

var lambdaFunction = builder.AddAWSLambdaFunction<Projects.LambdaCoreWebAPI>("lambda-core-web-api", 
    "LambdaCoreWebAPI::LambdaCoreWebAPI.LambdaEntryPoint::FunctionHandlerAsync");

builder.AddAWSAPIGatewayEmulator("APIGatewayEmulator", Aspire.Hosting.AWS.Lambda.APIGatewayType.HttpV2)
    .WithReference(lambdaFunction, Aspire.Hosting.AWS.Lambda.Method.Any, "/{proxy+}");
    
builder.Build().Run();