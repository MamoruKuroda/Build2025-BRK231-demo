// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license.

namespace MyOpenAIWebApi.Options;

/// <summary>
/// Configuration options for custom API settings
/// </summary>
public class CustomAPIOptions
{
    /// <summary>
    /// Base URL for the custom API
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// API key for authentication
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}