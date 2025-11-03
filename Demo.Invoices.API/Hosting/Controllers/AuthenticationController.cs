using Demo.Invoices.API.Hosting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Invoices.API.Hosting.Controllers;

/// <summary>
/// If API wants to define its own user/permission store (not recommended for production scenarios)
/// we need endpoint to issue user token so that frontend app can access other controllers/endpoints.
/// Usually flow goes like this:
/// - Frontend app displays login screen
/// - User provides login/password and 'api/authenticate' is called returning JWT token
/// - Forntend app stores that token and reuse it everytime it wants to access other endpoints (e.g. invoice/create)
/// * For production case instead of generating token through 'api/authenticate' endpoint we redirect user to authorization server and obtain token from there.
/// </summary>
public class AuthenticationController : ControllerBase
{
    private readonly FakeUserStore _userStore;

    public AuthenticationController(FakeUserStore userStore)
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