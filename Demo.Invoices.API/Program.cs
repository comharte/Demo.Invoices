global using Demo.Invoices.API;
global using Demo.Invoices.API.Application;
global using Demo.Invoices.API.Infrastructure.Repository;
global using Demo.Invoices.API.Infrastructure.API;

//Demo.Invoices.API.SampleCode.SampleCodeExecution.Run();

var builder = WebApplication.CreateBuilder(args);

//Using extension method to allow each layer register its own dependencies
builder.Services.AddInfrastructure()
    .AddApplication()
    .AddHosting();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();