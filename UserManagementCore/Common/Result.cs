namespace UserManagementCore.Common;

public abstract record Result<T>
{
    private Result() { }
    
    public static Result<T> Success(T value) => new SuccessType(value);
    public static Result<T> Failure(string error) => new FailureType(error);
    public static Result<T> NotFound(string message) => new NotFoundType(message);

    public sealed record SuccessType(T Value) : Result<T>;
    public sealed record FailureType(string Error) : Result<T>;
    public sealed record NotFoundType(string Message) : Result<T>;
}