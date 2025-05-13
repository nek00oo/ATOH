using UserManagementApplication.Abstractions;

namespace UserManagementApplication.Utils;

using BCrypt.Net;

public class PasswordEncoder : IPasswordEncoder
{
    public string HashPassword(string password)
    {
        return BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Verify(password, hashedPassword);
    }
}