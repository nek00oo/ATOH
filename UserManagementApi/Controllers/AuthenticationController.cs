using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementCore.Common;

namespace UserManagementServer.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthenticationController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    /// <param name="userRequest">Данные для входа</param>
    /// <returns>Устанавливает cookie с JWT токеном</returns>
    /// <response code="200">Успешная аутентификация</response>
    /// <response code="400">Неверные учетные данные</response>
    [HttpPost("login")]
    [AllowAnonymous]
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