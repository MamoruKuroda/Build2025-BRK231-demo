// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license.

namespace MyOpenAIWebApi.Services;

/// <summary>
/// Interface for managing assistants
/// </summary>
public interface IAssistantManager
{
    /// <summary>
    /// Gets an assistant by ID
    /// </summary>
    /// <param name="assistantId">The assistant ID</param>
    /// <returns>The assistant data</returns>
    Task<object?> GetAssistantAsync(string assistantId);

    /// <summary>
    /// Creates a new assistant
    /// </summary>
    /// <param name="assistantData">The assistant data</param>
    /// <returns>The created assistant</returns>
    Task<object> CreateAssistantAsync(object assistantData);
}

/// <summary>
/// In-memory implementation of assistant manager
/// </summary>
public class InMemoryAssistantManager : IAssistantManager
{
    private readonly Dictionary<string, object> _assistants = new();

    /// <summary>
    /// Gets an assistant by ID
    /// </summary>
    /// <param name="assistantId">The assistant ID</param>
    /// <returns>The assistant data</returns>
    public Task<object?> GetAssistantAsync(string assistantId)
    {
        _assistants.TryGetValue(assistantId, out var assistant);
        return Task.FromResult(assistant);
    }

    /// <summary>
    /// Creates a new assistant
    /// </summary>
    /// <param name="assistantData">The assistant data</param>
    /// <returns>The created assistant</returns>
    public Task<object> CreateAssistantAsync(object assistantData)
    {
        var assistantId = Guid.NewGuid().ToString();
        _assistants[assistantId] = assistantData;
        return Task.FromResult(assistantData);
    }
}