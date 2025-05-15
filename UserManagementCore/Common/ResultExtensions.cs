namespace UserManagementCore.Common;

public static class ResultExtensions
{
    public static T? TryGetValue<T>(this Result<T> result, out string message)
    {
        message = result switch
        {
            Result<T>.SuccessType success => "",
            Result<T>.SuccessCreateType successCreate => "",
            Result<T>.FailureType failure => failure.Error,
            Result<T>.NotFoundType notFound => notFound.Message,
            _ => throw new InvalidOperationException("Unknown Result type")
        };

        return result switch
        {
            Result<T>.SuccessType success => success.Value,
            Result<T>.SuccessCreateType successCreate => successCreate.Value,
            _ => default
        };
    }
}