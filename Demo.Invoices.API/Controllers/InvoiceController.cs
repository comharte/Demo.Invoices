using Microsoft.AspNetCore.Mvc;

namespace Demo.Invoices.API.Controllers;

[ApiController]
public class InvoiceController : ControllerBase
{
    public InvoiceController()
    {
    }

    [HttpGet("api/dev")]
    public IActionResult Dev(CancellationToken cancellationToken)
    {
        return Ok("Development endpoint is working.");
    }
}