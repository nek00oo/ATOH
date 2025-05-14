using Infrastructure.Data;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using UserManagementCore.Contracts;
using UserManagementCore.Exceptions;
using UserManagementCore.Models;
using UserManagementCore.Types;

namespace Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly UserDbContext _dbContext;
    private readonly UserPersistenceMapper _persistenceMapper;

    public UsersRepository(UserDbContext dbContext, UserPersistenceMapper persistenceMapper)
    {
        _dbContext = dbContext;
        _persistenceMapper = persistenceMapper;
    }
    
    public async Task<UserModel> CreateAsync(UserModel userModel)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Login == userModel.Login))
            throw new UniqueConstraintException($"Login {userModel.Login} already exists");
        
        var userEntity = _persistenceMapper.ToEntity(userModel);
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();

        return userModel;
    }

    public async Task<UserModel?> FindByLoginAsync(string login)
    {
        var userEntity = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login == login);
        
        if (userEntity is null)
            return null;

        var user = _persistenceMapper.ToDomain(userEntity);
        if (user is null)
            throw new InvalidOperationException("User is incorrect");
        
        return user;
    }

    public async Task<List<UserModel>> GetActiveUsersAsync()
    {
        var activeUsersEntities =  await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.RevokedOn == null)
            .OrderBy(u => u.CreatedOn)
            .ToListAsync();
        
        var activeUsers = activeUsersEntities
            .Select(_persistenceMapper.ToDomain)
            .Where(u => u != null)
            .Cast<UserModel>()
            .ToList();

        return activeUsers;
    }

    public async Task<List<UserModel>> GetUsersOlderThanAsync(int age)
    {
        var entities = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Birthday.HasValue && u.Birthday <= DateTime.UtcNow.AddYears(-age))
            .ToListAsync();
        
        return entities
            .Select(_persistenceMapper.ToDomain)
            .Where(u => u is not null)
            .Cast<UserModel>()
            .ToList();
    }

    public async Task<UserModel> UpdateProfileAsync(
        string login,
        string newName,
        Gender newGender,
        DateTime? newBirthday,
        string modifiedBy)
    {
        var userEntity =
            await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login)
            ?? throw new InvalidOperationException($"User with login {login} not found");

        userEntity.Name = newName;
        userEntity.Gender = newGender;
        userEntity.Birthday = newBirthday;
        userEntity.ModifiedOn = DateTime.UtcNow;
        userEntity.ModifiedBy = modifiedBy;

        await _dbContext.SaveChangesAsync();
        
        var user = _persistenceMapper.ToDomain(userEntity);
        if (user is null)
            throw new InvalidOperationException("User is incorrect");
        
        return user;
    }

    public async Task<UserModel> ChangePasswordAsync(string login, string newPasswordHash, string modifiedBy)
    {
        var userEntity =
            await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login)
            ?? throw new InvalidOperationException($"User with login {login} not found");
        
        userEntity.Password = newPasswordHash;
        userEntity.ModifiedOn = DateTime.UtcNow;
        userEntity.ModifiedBy = modifiedBy;
        
        await _dbContext.SaveChangesAsync();
        
        var user = _persistenceMapper.ToDomain(userEntity);
        if (user is null)
            throw new InvalidOperationException("User is incorrect");
        
        return user;
    }

    public async Task<UserModel> ChangeLoginAsync(string currentLogin, string newLogin, string modifiedBy)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Login == newLogin))
            throw new UniqueConstraintException($"Login {newLogin} already exists");
        
        var userEntity =
            await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == currentLogin)
            ?? throw new InvalidOperationException($"User with login {currentLogin} not found");
        
        userEntity.Login = newLogin;
        userEntity.ModifiedOn = DateTime.UtcNow;
        userEntity.ModifiedBy = modifiedBy;
        
        await _dbContext.SaveChangesAsync();
        
        var user = _persistenceMapper.ToDomain(userEntity);
        if (user is null)
            throw new InvalidOperationException("User is incorrect");
        
        return user;
    }

    public async Task<bool> SoftDeleteAsync(string login, string revokedBy)
    {
        var userEntity =
                await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login) 
                ?? throw new InvalidOperationException($"User with login {login} not found");

        if (userEntity.RevokedOn != null)
            throw new InvalidOperationException($"User already revoked on {userEntity.RevokedOn}");

        userEntity.RevokedOn = DateTime.UtcNow;
        userEntity.RevokedBy = revokedBy;

        await _dbContext.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> HardDeleteAsync(string login)
    {
        var userEntity =
            await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login)
            ?? throw new InvalidOperationException($"User with login {login} not found");

        _dbContext.Users.Remove(userEntity);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<UserModel> RestoreAsync(string login)
    {
        var userEntity =
            await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login)
            ?? throw new InvalidOperationException($"User with login {login} not found");

        userEntity.RevokedOn = null;
        userEntity.RevokedBy = null;

        await _dbContext.SaveChangesAsync();
        
        var user = _persistenceMapper.ToDomain(userEntity);
        if (user is null)
            throw new InvalidOperationException("User is incorrect");
        
        return user;
    }
}