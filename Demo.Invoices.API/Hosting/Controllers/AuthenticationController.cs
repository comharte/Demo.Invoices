using Demo.Invoices.API.Hosting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Invoices.API.Hosting.Controllers;

public class AuthenticationController : ControllerBase
{
    private readonly UserStore _userStore;

    public AuthenticationController(UserStore userStore)
    {
        _userStore = userStore;
    }

    [HttpPost("api/authenticate")]
    [AllowAnonymous]
    public IActionResult Authenticate([FromBody] LoginRequest request)
    {
        if (_userStore.TryAuthenticate(request.Login ?? string.Empty, request.Password ?? string.Empty, out var claims))
        {
            var token = _userStore.CreateToken(claims);
            return Ok(new { Token = token });
        }

        return Unauthorized();
    }
}

public class LoginRequest
{
    public string? Login { get; set; }
    public string? Password { get; set; }
}