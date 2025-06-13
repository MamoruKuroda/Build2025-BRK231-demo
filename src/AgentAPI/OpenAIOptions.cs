// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license.

namespace MyOpenAIWebApi.Options;

/// <summary>
/// Configuration options for Azure OpenAI services
/// </summary>
public class OpenAIOptions
{
    /// <summary>
    /// The endpoint URL for the Azure OpenAI service
    /// </summary>
    public string Endpoint { get; set; } = "https://ai-makurodaautogen207521401865.openai.azure.com/openai/deployments/gpt-4.1/chat/completions?api-version=2025-01-01-preview";

    /// <summary>
    /// The API key for authentication with Azure OpenAI
    /// </summary>
    public string Key { get; set; } = "6daPQpKkG4HxAM5NSpFgdMlUi4hh1w9b0gWBAbkQzFMzW2S8rfZLJQQJ99BEACHYHv6XJ3w3AAAAACOGAZ9u";

    /// <summary>
    /// The model to use (defaults to gpt-4o-mini)
    /// </summary>
    public string Model { get; set; } = "gpt-4.1";

    /// <summary>
    /// Whether to use API key authentication (true) or Azure AD authentication (false)
    /// </summary>
    public bool UseKeyAuth { get; set; } = true;
}