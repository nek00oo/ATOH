namespace UserManagementCore.Exceptions;

public class UserAlreadyRevokedException(DateTime revokedOn)
    : Exception($"User already revoked on {revokedOn:yyyy-MM-dd}");