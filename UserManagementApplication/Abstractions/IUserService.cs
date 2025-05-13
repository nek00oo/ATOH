using UserManagementApplication.Dto;
using UserManagementCore.Common;
using UserManagementCore.Models;

namespace UserManagementApplication.Abstractions;

public interface IUserService
{
    Task<Result<UserResponse>> CreateUserAsync(CreateUserDto createUserDto);
}