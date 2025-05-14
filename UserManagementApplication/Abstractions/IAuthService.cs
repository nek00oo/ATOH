namespace UserManagementApplication.Abstractions;

public interface IAuthService
{
    Task<string> Login(string login, string password);
}