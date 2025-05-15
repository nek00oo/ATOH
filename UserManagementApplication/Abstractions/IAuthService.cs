using UserManagementApplication.Dto;
using UserManagementCore.Common;

namespace UserManagementApplication.Abstractions;

public interface IAuthService
{
    Task<Result<LoginUserResponse>> Login(string login, string password);
}