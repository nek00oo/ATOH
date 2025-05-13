using UserManagementCore.Models;

namespace UserManagementCore.Contracts;

public interface IUsersRepository
{
    Task<UserModel?> GetByLoginAsync(string login);
    Task<List<UserModel>> GetActiveUsersAsync();
    Task<List<UserModel>> GetUsersOlderThanAsync(int age);
    Task<bool> IsLoginUniqueAsync(string login);
    Task<UserModel> CreateAsync(UserModel userModel);
    Task<UserModel> UpdateAsync(UserModel userModel);
    Task SoftDeleteAsync(string login, string revokedBy);
    Task RestoreAsync(string login);
}