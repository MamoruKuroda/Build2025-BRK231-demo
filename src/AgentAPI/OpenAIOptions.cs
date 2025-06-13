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
    public string Endpoint { get; set; } = "https://<Azure AI Foundy deployment name>.openai.azure.com/openai/deployments/gpt-4.1/chat/completions?api-version=2025-01-01-preview";

    /// <summary>
    /// The API key for authentication with Azure OpenAI
    /// </summary>
    public string Key { get; set; } = "<API Key>";

    /// <summary>
    /// The model to use (defaults to gpt-4o-mini)
    /// </summary>
    public string Model { get; set; } = "<Model Name>";

    /// <summary>
    /// Whether to use API key authentication (true) or Azure AD authentication (false)
    /// </summary>
    public bool UseKeyAuth { get; set; } = true;
}
