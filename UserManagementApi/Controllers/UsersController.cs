using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementServer.Extensions.Application;

namespace UserManagementServer.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>
    /// Создать нового пользователя
    /// </summary>
    /// <param name="userDto">Данные нового пользователя</param>
    /// <returns>Созданный пользователь</returns>
    /// <response code="201">Пользователь успешно создан</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется аутентификация</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
    {
        var currentUserLogin = User.FindFirst("Login")?.Value;
        
        if (currentUserLogin == null)
            return Unauthorized();
        
        var result = await userService.CreateUserAsync(userDto, currentUserLogin);
        return result.ToActionResult();
    }
    
    /// <summary>
    /// Получить пользователя по логину
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <returns>Информация о пользователе</returns>
    /// <response code="200">Пользователь найден</response>
    /// <response code="401">Требуется аутентификация</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpGet("{login}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetUserByLogin(string login)
    {
        var result = await userService.GetUserByLoginAsync(login);
        return result.ToActionResult();
    }
    
    /// <summary>
    /// Получить поток активных пользователей (streaming)
    /// </summary>
    /// <remarks>
    /// Возвращает поток пользователей в режиме реального времени.
    /// </remarks>
    /// <returns>Поток пользователей в формате JSON-стрима</returns>
    /// <response code="200">Начало потока данных</response>
    /// <response code="401">Требуется аутентификация</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    [HttpGet("active")]
    [Authorize(Policy = "AdminOnly")]
    public IAsyncEnumerable<UserResponse> GetActiveUsers()
    {
        return userService.GetActiveUsersAsync();
    }
    
    /// <summary>
    /// Получить пользователей старше указанного возраста (streaming)
    /// </summary>
    /// <remarks>
    /// Возвращает поток пользователей в режиме реального времени.
    /// </remarks>
    /// <returns>Поток пользователей в формате JSON-стрима</returns>
    /// <response code="200">Начало потока данных</response>
    /// <response code="401">Требуется аутентификация</response>
    /// <response code="403">Недостаточно прав</response>
    [HttpGet("older-than/{age}")]
    [ProducesResponseType(typeof(IAsyncEnumerable<UserResponse>), 200)]
    [ProducesResponseType(400)]
    public IAsyncEnumerable<UserResponse> GetUsersOlderThan([Range(0, 150)] int age)
    {
        return userService.GetUsersOlderThanStreamAsync(age);
    }
    
    /// <summary>
    /// Обновить профиль пользователя
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <param name="dto">Новые данные профиля</param>
    /// <returns>Обновленный профиль</returns>
    /// <response code="200">Профиль обновлен</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется аутентификация</response>
    /// <response code="403">Недостаточно прав</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpPatch("{login}/profile")]
    public async Task<IActionResult> UpdateProfile(string login, [FromBody] UpdateProfileDto dto)
    {
        var currentLogin = User.FindFirst("Login")?.Value;

        if (currentLogin == null)
            return Unauthorized();
        
        if (currentLogin != login && User.HasClaim("Admin", "true") is false)
            return Forbid();
        
        var result = await userService.UpdateProfileAsync(login, dto, currentLogin);
        return result.ToActionResult();
    }

    /// <summary>
    /// Изменить пароль пользователя
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <param name="dto">Данные для смены пароля</param>
    /// <returns>Результат операции</returns>
    /// <response code="200">Пароль изменен</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется аутентификация</response>
    /// <response code="403">Недостаточно прав</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpPatch("{login}/password")]
    public async Task<IActionResult> ChangePassword(string login, [FromBody] ChangePasswordDto dto)
    {
        var currentLogin = User.FindFirst("Login")?.Value;
        
        if (currentLogin == null)
            return Unauthorized();
        
        if (currentLogin != login && User.HasClaim("Admin", "true") is false)
            return Forbid();
        
        var result = await userService.ChangePasswordAsync(login, dto, currentLogin);
        return result.ToActionResult();
    }

    /// <summary>
    /// Изменить логин пользователя
    /// </summary>
    /// <param name="login">Текущий логин</param>
    /// <param name="dto">Данные для смены логина</param>
    /// <returns>Результат операции</returns>
    /// <response code="200">Логин изменен</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется аутентификация</response>
    /// <response code="403">Недостаточно прав</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpPatch("{login}/login")]
    public async Task<IActionResult> ChangeLogin(string login, [FromBody] ChangeLoginDto dto)
    {
        var currentLogin = User.FindFirst("Login")?.Value;
        
        if (currentLogin == null)
            return Unauthorized();
        
        if (currentLogin != login && User.HasClaim("Admin", "true") is false)
            return Forbid();
        
        var result = await userService.ChangeLoginAsync(login, dto, currentLogin);
        return result.ToActionResult();
    }
    
    /// <summary>
    /// Удалить пользователя
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <param name="softDelete">Флаг мягкого удаления (true по умолчанию)</param>
    /// <returns>Результат операции</returns>
    /// <response code="200">Пользователь удален</response>
    /// <response code="401">Требуется аутентификация</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    /// <response code="404">Пользователь не найден</response>
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
    
    /// <summary>
    /// Восстановить удаленного пользователя (очищает поля RevokedOn/RevokedBy)
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <returns>Восстановленный пользователь</returns>
    /// <response code="200">Пользователь успешно восстановлен</response>
    /// <response code="400">Ошибка при восстановлении</response>
    /// <response code="401">Требуется аутентификация</response>
    /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpPatch("{login}/restore")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RestoreUser(string login)
    {
        var result = await userService.RestoreUserAsync(login);
        return result.ToActionResult();
    }
}