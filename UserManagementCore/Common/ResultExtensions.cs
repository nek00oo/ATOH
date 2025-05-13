namespace UserManagementCore.Common;

public static class ResultExtensions
{
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Result<T> result,
        Func<T, Task<TResult>> onSuccess,
        Func<string, Task<TResult>> onFailure)
    {
        return result switch
        {
            Result<T>.SuccessType s => await onSuccess(s.Value),
            Result<T>.FailureType f => await onFailure(f.Error),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }
}