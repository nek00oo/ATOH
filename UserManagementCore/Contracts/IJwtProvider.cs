using UserManagementCore.Models;

namespace UserManagementCore.Contracts;

public interface IJwtProvider
{
    string GenerateToken(UserModel user);
}