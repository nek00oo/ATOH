using Microsoft.AspNetCore.Mvc;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;

namespace UserManagementServer.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest userRequest)
    {
        var token = await _authService.Login(userRequest.Login, userRequest.Password);
        
        Response.Cookies.Append("token", token);

        return Ok();
    }
}