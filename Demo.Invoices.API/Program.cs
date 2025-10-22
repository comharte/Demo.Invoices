global using System.Text.Json;
global using Demo.Invoices.API;
global using Demo.Invoices.API.Application;
global using Demo.Invoices.API.Hosting.Middleware;
global using Demo.Invoices.API.Infrastructure.Repository;
global using Demo.Invoices.API.Infrastructure.API;

//Demo.Invoices.API.SampleCode.SampleCodeExecution.Run();

var builder = WebApplication.CreateBuilder(args);

//Using extension method to allow each layer register its own dependencies
builder.Services.AddInfrastructure()
    .AddApplication(builder.Configuration)
    .AddHosting();

var env = builder.Configuration.GetValue<string>("env");

builder.Services.AddSingleton<InvoiceServiceConfiguration>(isc);

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    ///openapi/v1.json
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Configuration.GetValue<bool>("UseTeapotMiddleware"))
{
    app.UseMiddleware<TeapotMiddleware>();
}

app.UseAuthorization();

app.MapControllers();

app.Run();