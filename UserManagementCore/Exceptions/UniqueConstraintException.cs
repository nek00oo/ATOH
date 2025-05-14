namespace UserManagementCore.Exceptions;

public class UniqueConstraintException(string message) : Exception(message);