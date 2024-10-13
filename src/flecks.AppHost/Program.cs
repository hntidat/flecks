using flecks.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();

var redis = builder.AddRedis("redis");
var rabbit = builder.AddRabbitMQ("eventbus-rabbit");
//var mongo = builder.AddMongoDB("mongo");
var postgres = builder.AddPostgres("postgres")
    .WithImage("ankane/pgvector")
    .WithImageTag("latest");
var userPostgres = builder.AddPostgres("user-postgres")
    .WithImageTag("latest");

var userDb = userPostgres.AddDatabase("user-db");
var identityDb = userPostgres.AddDatabase("identity-db");
var mediaDb = postgres.AddDatabase("media-db");
var libraryDb = postgres.AddDatabase("library-db");
var streamingDb = postgres.AddDatabase("streaming-db");

builder.AddProject<Projects.Identity_API>("identity-api")
    .WithReference(redis)
    .WithReference(rabbit)
    .WithReference(identityDb);

builder.AddProject<Projects.Library_API>("library-api")
    .WithReference(redis)
    .WithReference(rabbit)
    .WithReference(libraryDb);

builder.AddProject<Projects.Media_API>("media-api")
    .WithReference(redis)
    .WithReference(rabbit)
    .WithReference(mediaDb);

builder.AddProject<Projects.Streaming_API>("streaming-api")
    .WithReference(redis)
    .WithReference(rabbit)
    .WithReference(streamingDb);

builder.AddProject<Projects.User_API>("user-api")
    .WithReference(redis)
    .WithReference(rabbit)
    .WithReference(userDb);

builder.AddNpmApp("angular", "../WepApp.Angular")
    .WithHttpEndpoint(env: "PORT");
    
//.PublishAsDockerFile();



builder.AddProject<Projects.ApiGateway>("apigateway")
    .WithExternalHttpEndpoints();

    //.PublishAsDockerFile();



builder.Build().Run();
