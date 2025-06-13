// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyOpenAIWebApi.Options;
using MyOpenAIWebApi.Services;
using MyOpenAIWebApi.Helpers;
using MyOpenAIWebApi.Hubs;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.JsonWebTokens;

/// <summary>
/// Application entry point and configuration for the OpenAI Web API.
/// This file sets up the ASP.NET Core application, configures services,
/// and establishes the middleware pipeline.
/// </summary>

// Create the web application builder
var builder = WebApplication.CreateBuilder(args);

// Configure application settings from appsettings.json files
builder.Services.Configure<OpenAIOptions>(builder.Configuration.GetSection("AzureOpenAI"));
builder.Services.Configure<CustomAPIOptions>(builder.Configuration.GetSection("CustomAPI"));
builder.Services.Configure<RAGOptions>(builder.Configuration.GetSection("RAG"));

// Add controllers for handling API endpoints
builder.Services.AddControllers();


/// <summary>
/// Configure JWT Authentication using Microsoft Entra ID (formerly Azure Active Directory)
/// This sets up the JWT Bearer authentication scheme for the API.
/// </summary>
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        // Configure authority for token validation and key discovery
        options.Authority = builder.Configuration["Entra:ValidationAuthority"];
          // Configure token validation parameters to accept multiple issuers and audiences
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            // Accept multiple issuer formats from Entra External ID
            ValidIssuers = new[]
            {
                builder.Configuration["Entra:ValidIssuer"], // CIAM format
                $"https://sts.windows.net/{builder.Configuration["Entra:TenantId"]}/", // STS format
                $"https://e69542c0-54c0-41fa-9380-a8f5a82a7f6d.ciamlogin.com/e69542c0-54c0-41fa-9380-a8f5a82a7f6d/v2.0" // ID token format
            },
            ValidateAudience = true,
            // Accept multiple audiences: WebApp ClientId and Microsoft Graph
            ValidAudiences = new[]
            {
                builder.Configuration["Entra:ValidAudience"], // WebApp ClientId
                "00000003-0000-0000-c000-000000000000", // Microsoft Graph
                builder.Configuration["Entra:ClientId"] // AgentAPI ClientId
            },            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // Custom issuer signing key resolver for multiple authorities
            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                var authorities = new[]
                {
                    builder.Configuration["Entra:ValidationAuthority"],
                    $"https://sts.windows.net/{builder.Configuration["Entra:TenantId"]}/",
                    $"https://login.microsoftonline.com/{builder.Configuration["Entra:TenantId"]}/v2.0"
                };

                var keys = new List<SecurityKey>();
                foreach (var authority in authorities)
                {
                    try
                    {
                        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                            $"{authority}/.well-known/openid-configuration",
                            new OpenIdConnectConfigurationRetriever());
                        var config = configManager.GetConfigurationAsync().Result;
                        keys.AddRange(config.SigningKeys);
                    }
                    catch (Exception ex)
                    {
                        // Log but continue with other authorities
                        Console.WriteLine($"Failed to retrieve keys from {authority}: {ex.Message}");
                    }
                }
                return keys;
            }
        };
        
        options.SaveToken = true;          // Save the token for later retrieval
        options.RequireHttpsMetadata = true; // Require HTTPS for metadata endpoints
          // Configure event handlers for token processing
        options.Events = new JwtBearerEvents
        {
            // Event handler for extracting the token from the request
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("OnMessageReceived: Processing request for path: {Path}", context.HttpContext.Request.Path);
                
                // First try to get token from Authorization header (for HTTP and WebSocket negotiation)
                string? token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                logger.LogInformation("OnMessageReceived: Token from Authorization header: {HasToken}", !string.IsNullOrEmpty(token));

                //TODO: Make sure if this makes sense to keep
                // If token not found in header, try query string (as fallback for WebSockets)
                if (string.IsNullOrEmpty(token))
                {
                    token = context.Request.Query["access_token"];
                    logger.LogInformation("OnMessageReceived: Token from query string: {HasToken}", !string.IsNullOrEmpty(token));
                }
                  // Check if the request is for the SignalR hub and set the token
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/assistantHub"))
                {
                    // Store token in context for SignalR authentication
                    context.Token = token;
                    logger.LogInformation("OnMessageReceived: Token set for SignalR hub");
                    
                    // Log token details for debugging
                    try
                    {
                        var handler = new JsonWebTokenHandler();
                        var jsonToken = handler.ReadJsonWebToken(token);
                        logger.LogInformation("JWT Token Details - Issuer: {Issuer}, Audience: {Audience}, KeyId: {KeyId}", 
                            jsonToken.Issuer, 
                            string.Join(", ", jsonToken.Audiences), 
                            jsonToken.Kid);
                        logger.LogInformation("JWT Token Algorithm: {Algorithm}", jsonToken.Alg);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to parse JWT token");
                    }
                }
                
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "JWT Authentication failed for path: {Path}", context.HttpContext.Request.Path);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("JWT Token validated successfully for path: {Path}", context.HttpContext.Request.Path);
                return Task.CompletedTask;
            }
        };
    });

/// <summary>
/// Configure SignalR for real-time communication between clients and server.
/// SignalR enables bi-directional communication for streaming assistant responses.
/// </summary>
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;               // Enable detailed errors for debugging
    options.MaximumReceiveMessageSize = 102400;        // Set maximum message size to 100 KB
});

/// <summary>
/// Configure Cross-Origin Resource Sharing (CORS) to allow browser-based clients
/// from different origins to interact with this API.
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {        // Get allowed origins from configuration or use default localhost value
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                            ?? new[] { "http://localhost:5118" }; // Fallback default
        
        // Configure CORS policy with required options
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()            // Allow all HTTP headers
              .AllowAnyMethod()            // Allow all HTTP methods (GET, POST, etc.)
              .AllowCredentials()          // Allow credentials (required for SignalR)
              .SetIsOriginAllowed(_ => true) // For development - set more restrictive in production
              .WithExposedHeaders("Content-Disposition"); // Expose specific headers to the client
    });
});

/// <summary>
/// Register services required for the application in the dependency injection container.
/// </summary>

// Register TokenHelpers for authenticating with Microsoft Entra ID
builder.Services.AddSingleton<TokenHelpers>(sp =>
{
    // Get Microsoft Entra ID configuration from app settings
    var config = sp.GetRequiredService<IConfiguration>();
    var tenantId = config["Entra:TenantId"] ?? "";
    var clientId = config["Entra:ClientId"] ?? "";
    var clientSecret = config["Entra:ClientSecret"] ?? "";
    var authority = config["Entra:Authority"] ?? "";
    
    return new TokenHelpers(tenantId, clientId, clientSecret, authority);
});

// Register application services in the dependency injection container
builder.Services.AddSingleton<CustomAPIHelper>();      // Helper for custom API operations
builder.Services.AddMemoryCache();                     // In-memory cache for application data
builder.Services.AddSingleton<IAssistantManager, InMemoryAssistantManager>(); // Assistant management service

/// <summary>
/// Configure Swagger for API documentation and exploration
/// </summary>
builder.Services.AddEndpointsApiExplorer();  // API explorer for endpoint discovery
builder.Services.AddSwaggerGen();            // Swagger generator for API documentation

// Build the web application
var app = builder.Build();

/// <summary>
/// Configure the HTTP request pipeline with middleware components
/// </summary>

// Enable Swagger UI only in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();          // Enable Swagger endpoint for OpenAPI specification
    app.UseSwaggerUI();        // Enable Swagger UI for interactive API documentation
}

// Configure the HTTP request processing pipeline in the correct order
app.UseCors();                // Apply CORS policies first
app.UseRouting();             // Set up routing for endpoint matching
app.UseAuthentication();      // Authenticate users based on credentials
app.UseAuthorization();       // Authorize users based on claims/roles
app.UseHttpsRedirection();    // Redirect HTTP requests to HTTPS

// Map endpoints to controllers and SignalR hubs
app.MapControllers();                         // Map controller actions to routes
app.MapHub<AssistantHub>("/assistantHub");    // Map SignalR hub to its endpoint

// Start the application
app.Run();
