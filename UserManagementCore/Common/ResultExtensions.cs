namespace UserManagementCore.Common;

public static class ResultExtensions
{
    public static T? TryGetValue<T>(this Result<T> result, out string error)
    {
        error = "";
    
        if (result is Result<T>.SuccessType success)
            return success.Value;
        
        if (result is Result<T>.FailureType failure)
        {
            error = failure.Error;
            return default;
        }

        throw new InvalidOperationException("Unknown Result type");
    }
}