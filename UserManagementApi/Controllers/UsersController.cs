using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementServer.Extensions;

namespace UserManagementServer.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
    {
        var result = await _userService.CreateUserAsync(userDto);
        return result.ToActionResult();
    }
}