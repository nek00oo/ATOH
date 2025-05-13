using UserManagementCore.Types;

namespace UserManagementApplication.Dto;

public record UserResponse(
    Guid Id,
    string Login,
    Gender Gender,
    DateTime? Birthday,
    bool Admin);