using UserManagementCore.Types;

namespace UserManagementApplication.Dto;

public record UpdateProfileDto(string Name, Gender Gender, DateTime? Birthday);