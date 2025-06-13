// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license.

namespace MyOpenAIWebApi.Options;

/// <summary>
/// Configuration options for RAG (Retrieval-Augmented Generation) functionality
/// </summary>
public class RAGOptions
{
    /// <summary>
    /// Search service endpoint
    /// </summary>
    public string SearchEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Search service API key
    /// </summary>
    public string SearchApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Search index name
    /// </summary>
    public string SearchIndexName { get; set; } = string.Empty;
}