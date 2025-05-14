using UserManagementCore.Common;

namespace UserManagementApplication.Abstractions;

public interface IAuthService
{
    Task<Result<string>> Login(string login, string password);
}