using UserManagementApplication.Dto;
using UserManagementCore.Models;

namespace UserManagementApplication.Mappers;

public class UserMapper
{
    public UserResponse ToResponse(UserModel user)
    {
        return new UserResponse(
            user.Id,
            user.Login,
            user.Gender,
            user.Birthday,
            user.Admin
        );
    }
}