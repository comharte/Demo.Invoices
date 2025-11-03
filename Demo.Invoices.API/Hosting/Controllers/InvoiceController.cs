using Demo.Invoices.API.Hosting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Invoices.API.Hosting.Controllers;

//https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-9.0

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

    //All authorization requirements must be met to access this endpoint
    //If for example we would like to add another requirement like 'MustBeOver18' we could just add another Authorize attribute here without changing existing ones
    [Authorize(Roles = "Developer")]
    [AuthorizePermission("Access.Invoice.Dev.Resources")]
    [HttpGet("api/dev")]
    public IActionResult Dev(bool withException = false, CancellationToken cancellationToken = default)
    {
        if (withException)
        {
            throw new Exception("This is a test exception from /api/dev endpoint.");
        }

        return Ok("Development endpoint is working.");
    }

    [HttpGet("api/invoices/{id}")]
    public async Task<IActionResult> GetInvoiceAsync(int id, CancellationToken cancellationToken)
    {
        var invoice = await _service.GetInvoiceAsync(id, cancellationToken);

        return Ok(invoice);
    }

    [HttpPost("api/invoices")]
    public async Task<IActionResult> ModifyInvoiceAsync(InvoiceModel model, CancellationToken cancellationToken)
    {
        model = await _service.ModifyInvoiceAsync(model, cancellationToken);

        return Ok(model);
    }

    [HttpDelete("api/invoices/{id}")]
    public async Task<IActionResult> DeleteInvoiceAsync(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteInvoiceAsync(id, cancellationToken);
        return NoContent();
    }


    [HttpGet("api/customers")]
    public async Task<IActionResult> GetAvailableCustomersAsync(CancellationToken cancellationToken)
    {
        var customers = await _service.GetAvailableCustomersAsync(cancellationToken);

        return Ok(customers);
    }

    [HttpGet("api/currencies")]
    public async Task<IActionResult> GetAvailableCurrenciesAsync(CancellationToken cancellationToken)
    {
        var currencies = await _service.GetAvailableCurrenciesAsync(cancellationToken);

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
        //IInvoiceRepository invoiceRepository = new InvoiceRepository();

        //In order to create invoice service at this point I need to know all it dependencies and how to create them
        //I am lucky because CurrencyApiClient, CustomerApiClient, InvoiceRepository are not depending on any other services (parameterless constructors)
        //In real life scenario repository will depend on underlying database implementation - as we add that implementatnion we will need to update this code as well
        //API will probably depend on http clients - same example.
        //And this is just one place where we need to create InvoiceService - in real life scenario there will be many places like this. All of them needs to be updated when dependencies change
        return new InvoiceService(currencyApiClient, customerApiClient, null, null);
    }
}