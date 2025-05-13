using UserManagementCore.Types;

namespace UserManagementApplication.Dto;

public record CreateUserDto(
    string Login,
    string Password,
    string Name,
    Gender Gender,
    DateTime? Birthday,
    bool Admin);