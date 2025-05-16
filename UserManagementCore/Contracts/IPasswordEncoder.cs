namespace UserManagementCore.Contracts;

public interface IPasswordEncoder
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}