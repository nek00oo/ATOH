using Microsoft.AspNetCore.Mvc;
using UserManagementCore.Common;

namespace UserManagementServer.Extensions.Application;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        return result switch
        {
            Result<T>.SuccessType success => new OkObjectResult(success.Value),
            Result<T>.FailureType failure => new BadRequestObjectResult(new { error = failure.Error }),
            Result<T>.NotFoundType notFound => new NotFoundObjectResult(new { message = notFound.Message }),
            _ => new StatusCodeResult(500)
        };
    }
}