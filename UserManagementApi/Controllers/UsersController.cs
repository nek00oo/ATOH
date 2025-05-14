using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementServer.Extensions;

namespace UserManagementServer.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Создать нового пользователя", 
        Description = "Требуются права администратора")]
    [SwaggerResponse(201, "Пользователь создан", typeof(UserResponse))]
    [SwaggerResponse(400, "Некорректные данные пользователя")]
    [SwaggerResponse(401, "Требуется аутентификация")]
    [SwaggerResponse(403, "Недостаточно прав")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
    {
        var currentUserLogin = User.FindFirst("Login")?.Value;
        
        if (currentUserLogin == null)
            return Unauthorized();
        
        var result = await userService.CreateUserAsync(userDto, currentUserLogin);
        return result.ToActionResult();
    }
    
    [HttpGet("{login}")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(
        Summary = "Получить пользователя по логину",
        Description = "Требуются права администратора. Возвращает полную информацию о пользователе.")]
    [SwaggerResponse(200, "Информация о пользователе", typeof(UserResponse))]
    [SwaggerResponse(401, "Требуется аутентификация")]
    [SwaggerResponse(403, "Недостаточно прав (требуется роль Admin)")]
    [SwaggerResponse(404, "Пользователь не найден")]
    public async Task<IActionResult> GetUserByLogin(string login)
    {
        var result = await userService.GetUserByLoginAsync(login);
        return result.ToActionResult();
    }
    
    [HttpGet("active")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetActiveUsers()
    {
        var result = await userService.GetActiveUsersAsync();
        return result.ToActionResult();
    }
    
    [HttpGet("older-than/{age}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetUsersOlderThan(int age)
    {
        var result = await userService.GetUsersOlderThanAsync(age);
        return result.ToActionResult();
    }
    
    [HttpPatch("{login}/profile")]
    public async Task<IActionResult> UpdateProfile(string login, [FromBody] UpdateProfileDto dto)
    {
        var currentLogin = User.FindFirst("Login")?.Value;

        if (currentLogin == null)
            return Unauthorized();
        
        if (currentLogin != login && User.IsInRole("Admin") is false)
            return Forbid();
        
        var result = await userService.UpdateProfileAsync(login, dto, currentLogin);
        return result.ToActionResult();
    }

    [HttpPatch("{login}/password")]
    public async Task<IActionResult> ChangePassword(string login, [FromBody] ChangePasswordDto dto)
    {
        var currentLogin = User.FindFirst("Login")?.Value;
        
        if (currentLogin == null)
            return Unauthorized();
        
        if (currentLogin != login && User.IsInRole("Admin") is false)
            return Forbid();
        
        var result = await userService.ChangePasswordAsync(login, dto, currentLogin);
        return result.ToActionResult();
    }

    [HttpPatch("{login}/login")]
    public async Task<IActionResult> ChangeLogin(string login, [FromBody] ChangeLoginDto dto)
    {
        var currentLogin = User.FindFirst("Login")?.Value;
        
        if (currentLogin == null)
            return Unauthorized();
        
        if (currentLogin != login && User.IsInRole("Admin") is false)
            return Forbid();
        
        var result = await userService.ChangeLoginAsync(login, dto, currentLogin);
        return result.ToActionResult();
    }
    
    [HttpDelete("{login}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteUser(string login, [FromQuery] bool softDelete = true)
    {
        var currentUserLogin = User.FindFirst("Login")?.Value;
        
        if (currentUserLogin == null)
            return Unauthorized();
        
        var result = await userService.DeleteUserAsync(login, softDelete, currentUserLogin);
        return result.ToActionResult();
    }
}