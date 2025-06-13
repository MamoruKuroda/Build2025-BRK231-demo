// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license.

namespace WoodgroveGroceriesApi.Middleware;

/// <summary>
/// Middleware for audit logging
/// </summary>
public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the AuditLoggingMiddleware class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger instance</param>
    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>Task representing the async operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Log the request
        _logger.LogInformation("Request: {Method} {Path} from {RemoteIp}", 
            context.Request.Method, 
            context.Request.Path, 
            context.Connection.RemoteIpAddress);

        // Call the next middleware
        await _next(context);

        // Log the response
        _logger.LogInformation("Response: {StatusCode} for {Method} {Path}", 
            context.Response.StatusCode,
            context.Request.Method, 
            context.Request.Path);
    }
}

/// <summary>
/// Extension methods for audit logging middleware
/// </summary>
public static class AuditLoggingExtensions
{
    /// <summary>
    /// Adds audit logging middleware to the pipeline
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuditLoggingMiddleware>();
    }
}