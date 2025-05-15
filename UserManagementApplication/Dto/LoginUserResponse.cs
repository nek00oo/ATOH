namespace UserManagementApplication.Dto;

public record LoginUserResponse(UserResponse User, string JwtToken);