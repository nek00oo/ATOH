namespace UserManagementApplication.Dto;

public record ChangePasswordDto(string? OldPassword, string NewPassword);