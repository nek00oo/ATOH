using Infrastructure.Entities;
using UserManagementCore.Common;
using UserManagementCore.Models;

namespace Infrastructure.Mappers;

public class UserPersistenceMapper
{
    public UserModel? ToDomain(UserEntity entity)
    {
        return UserModel.Create(
                entity.Id,
                entity.Login,
                entity.Password,
                entity.Name,
                entity.Gender,
                entity.Birthday ?? DateTime.MinValue,
                entity.Admin,
                entity.CreatedOn,
                entity.CreatedBy,
                entity.ModifiedOn,
                entity.ModifiedBy,
                entity.RevokedOn,
                entity.RevokedBy) switch
            {
                Result<UserModel>.SuccessType success => success.Value,
                _ => null
            };
    }

    public UserEntity ToEntity(UserModel model) => new UserEntity
    {
        Id = model.Id,
        Login = model.Login,
        Password = model.Password,
        Name = model.Name,
        Gender = model.Gender,
        Birthday = model.Birthday,
        Admin = model.Admin,
        CreatedOn = model.CreatedOn,
        CreatedBy = model.CreatedBy,
        ModifiedOn = model.ModifiedOn,
        ModifiedBy = model.ModifiedBy,
        RevokedOn = model.RevokedOn,
        RevokedBy = model.RevokedBy
    };
}
