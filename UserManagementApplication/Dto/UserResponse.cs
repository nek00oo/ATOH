using UserManagementCore.Types;

namespace UserManagementApplication.Dto;

public record UserResponse(
    string Login,
    string Name,
    Gender Gender,
    DateTime? Birthday,
    DateTime? RevokedOn);