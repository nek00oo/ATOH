using UserManagementCore.Models;
using UserManagementCore.Types;

namespace UserManagementCore.Contracts;

public interface IUsersRepository
{
    Task<UserModel> CreateAsync(UserModel userModel);
    
    Task<UserModel?> FindByLoginAsync(string login);
    
    Task<List<UserModel>> GetActiveUsersAsync();
    
    Task<List<UserModel>> GetUsersOlderThanAsync(int age);
    
    Task<UserModel> UpdateProfileAsync(
        string login, 
        string newName,
        Gender newGender,
        DateTime? newBirthday,
        string modifiedBy);
    
    Task<UserModel> ChangePasswordAsync(
        string login, 
        string newPasswordHash,
        string modifiedBy);
    
    Task<UserModel> ChangeLoginAsync(
        string currentLogin, 
        string newLogin,
        string modifiedBy);
    
    Task<bool> SoftDeleteAsync(string login, string revokedBy);

    Task<bool> HardDeleteAsync(string login);
    
    Task<UserModel> RestoreAsync(string login);
}