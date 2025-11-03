global using Demo.Invoices.API;
global using Demo.Invoices.API.Application;
global using Demo.Invoices.API.Hosting.Middleware;
global using Demo.Invoices.API.Infrastructure.API;
global using Demo.Invoices.API.Infrastructure.Repository;
global using System.Text.Json;

//Uncomment to run sample code
//Demo.Invoices.API.SampleCode.SampleCodeExecution.Run();

var builder = WebApplication.CreateBuilder(args);

//Using extension method to allow each layer register its own dependencies
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddHosting(builder.Configuration);

var app = builder.Build();

await app.ConfigureHostingPipeline();

app.Run();