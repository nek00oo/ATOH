using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementCore.Common;
using UserManagementServer.Extensions;

namespace UserManagementServer.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthenticationController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    /// <returns>Устанавливает cookie с JWT токеном</returns>
    /// <response code="200">Успешная аутентификация</response>
    /// <response code="400">Неверные учетные данные</response>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
    {
        var authResult = await authService.Login(userDto.Login, userDto.Password);
        if (authResult is Result<LoginUserResponse>.SuccessType success)
            Response.Cookies.Append("token", success.Value.JwtToken);
            
        return authResult.ToActionResult();
    }
}