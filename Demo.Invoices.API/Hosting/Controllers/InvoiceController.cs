using Microsoft.AspNetCore.Mvc;

namespace Demo.Invoices.API.Hosting.Controllers;

[ApiController]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _service;

    // Using Dependency Injection to get service instance
    // While creating InvoiceController DI Container inspects constructor parameters
    // If it finds registration (services.AddScoped<IInvoiceService,InvoiceService>()) it uses that definition to create instance of InvoiceService and pass it to the controller constructor
    // If we start adding more dependencies (diffrent business services), DI will automatically resolve them as long as they are registered in the container
    // So we really don't need to know how InvoiceService is created - DI Container will do it for us base on registration definition
    public InvoiceController(IInvoiceService service)
    {
        _service = service;
    }

    [HttpGet("api/dev")]
    public IActionResult Dev(CancellationToken cancellationToken)
    {
        return Ok("Development endpoint is working.");
    }

    [HttpGet("api/invoices/customers")]
    public async Task<IActionResult> GetAvailableCustomersAsync(CancellationToken cancellationToken)
    {
        var currencies = await _service.GetAvailableCustomersAsync(cancellationToken);
        return Ok(currencies);
    }


    [HttpGet("api/invoices/customers/withoutDependencyInjection")]
    public async Task<IActionResult> GetAvailableCustomersAsyncWithoutDependencyInjection(CancellationToken cancellationToken)
    {
        var service = CreateInvoiceServiceWithoutDI();
        var currencies = await service.GetAvailableCustomersAsync(cancellationToken);
        return Ok(currencies);
    }

    private static IInvoiceService CreateInvoiceServiceWithoutDI()
    {
        ICurrencyApiClient currencyApiClient = new CurrencyApiClient();
        ICustomerApiClient customerApiClient = new CustomerApiClient();
        IInvoiceRepository invoiceRepository = new InvoiceRepository();

        //In order to create invoice service at this point I need to know all it dependencies and how to create them
        //I am lucky because CurrencyApiClient, CustomerApiClient, InvoiceRepository are not depending on any other services (parameterless constructors)
        //In real life scenario repository will depend on underlying database implementation - as we add that implementatnion we will need to update this code as well
        //API will probably depend on http clients - same example.
        //And this is just one place where we need to create InvoiceService - in real life scenario there will be many places like this. All of them needs to be updated when dependencies change
        return new InvoiceService(currencyApiClient, customerApiClient, invoiceRepository);
    }
}