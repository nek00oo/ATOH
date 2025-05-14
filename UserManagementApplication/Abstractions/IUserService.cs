using UserManagementApplication.Dto;
using UserManagementCore.Common;
using UserManagementCore.Models;

namespace UserManagementApplication.Abstractions;

public interface IUserService
{
    Task<Result<UserResponse>> CreateUserAsync(CreateUserDto createUserDto, string createdBy);

    Task<Result<UserResponse>> GetUserByLoginAsync(string login);

    Task<Result<List<UserResponse>>> GetActiveUsersAsync();

    Task<Result<List<UserResponse>>> GetUsersOlderThanAsync(int age);

    Task<Result<UserResponse>> UpdateProfileAsync(string login, UpdateProfileDto dto, string modifiedBy);

    Task<Result<UserResponse>> ChangePasswordAsync(string login, ChangePasswordDto dto, string modifiedBy);

    Task<Result<UserResponse>> ChangeLoginAsync(string currentLogin, ChangeLoginDto dto, string modifiedBy);

    Task<Result<bool>> DeleteUserAsync(string login, bool softDelete, string revokedBy);

    Task<Result<UserResponse>> RestoreUserAsync(string login);
}