namespace Demo.Invoices.API;

/// <remarks>
/// Each method (AddApplication, AddInfrastructure, AddHosting) is responsible for registering dependencies related to its respective layer.
/// Usually, these methods would be placed in their respective projects (e.g., Application, Infrastructure, Hosting).
/// </remarks>
public static class Registration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var invoiceServiceConfiguration = configuration.GetSection("InvoiceServiceConfiguration").Get<InvoiceServiceConfiguration>();

        if (invoiceServiceConfiguration == null)
        {
            throw new Exception("InvoiceServiceConfiguration is not configured properly.");
        }
        services.AddSingleton<InvoiceServiceConfiguration>(invoiceServiceConfiguration);
        services.AddScoped<IInvoiceService, InvoiceService>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<ICurrencyApiClient, CurrencyApiClient>();
        services.AddScoped<ICustomerApiClient, CustomerApiClient>();

        return services;
    }

    public static IServiceCollection AddHosting(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        services.AddSwaggerGen();
        services.AddControllers();
        return services;
    }
}