using Infrastructure.Entities;
using UserManagementCore.Models;

namespace Infrastructure.Mappers;

public interface IUserPersistenceMapper
{
    UserModel? ToDomain(UserEntity entity);
    UserEntity ToEntity(UserModel model);
}