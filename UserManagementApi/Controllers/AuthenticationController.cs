using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementCore.Common;

namespace UserManagementServer.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthenticationController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Аутентификация пользователя", 
        Description = "Возвращает JWT токен и устанавливает cookie")]
    [SwaggerResponse(200, "Успешная аутентификация")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest userRequest)
    {
        var authResult = await authService.Login(userRequest.Login, userRequest.Password);
    
        switch (authResult)
        {
            case Result<string>.SuccessType success:
                Response.Cookies.Append("token", success.Value);
                return Ok();
            case Result<string>.FailureType fail:
                return BadRequest(fail.Error);
            default:
                throw new InvalidOperationException("Unexpected result type from auth service");
        }
    }
}