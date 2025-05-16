using UserManagementApplication.Dto;
using UserManagementCore.Models;

namespace UserManagementApplication.Abstractions;

public interface IUserResponseMapper
{
    UserResponse ToResponse(UserModel user);
}