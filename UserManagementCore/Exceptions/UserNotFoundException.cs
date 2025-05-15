namespace UserManagementCore.Exceptions;

public class UserNotFoundException(string login) : Exception($"User '{login}' not found");