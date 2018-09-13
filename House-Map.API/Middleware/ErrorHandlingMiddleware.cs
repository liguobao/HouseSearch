using HouseMapAPI.CommonException;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;

    private readonly ILogger _logger;

    public ErrorHandlingMiddleware(RequestDelegate next,
     ILogger<ErrorHandlingMiddleware> logger)
    {
        this.next = next;
        this._logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(-1, ex, ex.Message);
            if (ex is TokenInvalidException)
            {
                await HandleExceptionAsync(context, ex.Message, 401);
            }
            else if (ex is NotFoundException)
            {
                await HandleExceptionAsync(context, ex.Message, 404);
            }
            else if (ex is UnProcessableException)
            {
                await HandleExceptionAsync(context, ex.Message, 422);
            }
            else
            {
                await HandleExceptionAsync(context, ex.Message);
            }

        }
    }

    private static Task HandleExceptionAsync(HttpContext context, string error, int status = 500)
    {
        var data = new { code = -1, error = error };
        var result = JsonConvert.SerializeObject(data);
        context.Response.ContentType = "application/json;charset=utf-8";
        context.Response.StatusCode = status;
        return context.Response.WriteAsync(result);
    }
}

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}