using UserManagementApplication.Dto;
using UserManagementCore.Models;

namespace UserManagementApplication.Mappers;

public class UserResponseMapper
{
    public UserResponse ToResponse(UserModel user)
    {
        return new UserResponse(
            user.Login,
            user.Name,
            user.Gender,
            user.Birthday,
            user.RevokedOn
        );
    }
}