using Infrastructure.Data;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using UserManagementCore.Contracts;
using UserManagementCore.Models;

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

    public async Task<UserModel?> GetByLoginAsync(string login)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (userEntity == null)
            return null;

        return _persistenceMapper.ToDomain(userEntity);
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
            .Where(u => u.Birthday.HasValue && u.Birthday <= DateTime.UtcNow.AddYears(-age))
            .ToListAsync();
        
        return entities
            .Select(_persistenceMapper.ToDomain)
            .Where(u => u is not null)
            .Cast<UserModel>()
            .ToList();
    }

    public async Task<bool> IsLoginUniqueAsync(string login)
    {
        return !await _dbContext.Users.AnyAsync(u => u.Login == login);
    }

    public async Task<UserModel> CreateAsync(UserModel userModel)
    {
        var userEntity = _persistenceMapper.ToEntity(userModel);
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();

        return userModel;
    }

    public async Task<UserModel> UpdateAsync(UserModel userModel)
    {
        var existingEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userModel.Id);
        if (existingEntity == null)
            throw new InvalidOperationException("User not found");

        var updatedEntity = _persistenceMapper.ToEntity(userModel);
        
        _dbContext.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);

        await _dbContext.SaveChangesAsync();

        return userModel;
    }

    public async Task SoftDeleteAsync(string login, string revokedBy)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (userEntity == null)
            throw new InvalidOperationException("User not found");

        userEntity.RevokedOn = DateTime.UtcNow;
        userEntity.RevokedBy = revokedBy;

        await _dbContext.SaveChangesAsync();
    }

    public async Task RestoreAsync(string login)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (userEntity == null)
            throw new InvalidOperationException("User not found");

        userEntity.RevokedOn = null;
        userEntity.RevokedBy = null;

        await _dbContext.SaveChangesAsync();
    }
}