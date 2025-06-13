// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc;
using MyOpenAIWebApi.Services;

namespace MyOpenAIWebApi.Controllers;

/// <summary>
/// Assistant controller for managing AI assistants
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AssistantController : ControllerBase
{
    private readonly IAssistantManager _assistantManager;

    /// <summary>
    /// Initializes a new instance of the AssistantController class
    /// </summary>
    /// <param name="assistantManager">The assistant manager</param>
    public AssistantController(IAssistantManager assistantManager)
    {
        _assistantManager = assistantManager;
    }

    /// <summary>
    /// Gets an assistant by ID
    /// </summary>
    /// <param name="id">The assistant ID</param>
    /// <returns>The assistant data</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssistant(string id)
    {
        var assistant = await _assistantManager.GetAssistantAsync(id);
        if (assistant == null)
        {
            return NotFound($"Assistant with ID '{id}' not found.");
        }
        return Ok(assistant);
    }

    /// <summary>
    /// Creates a new assistant
    /// </summary>
    /// <param name="assistantData">The assistant data</param>
    /// <returns>The created assistant</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAssistant([FromBody] object assistantData)
    {
        var createdAssistant = await _assistantManager.CreateAssistantAsync(assistantData);
        return Ok(createdAssistant);
    }

    /// <summary>
    /// Gets the list of all assistants
    /// </summary>
    /// <returns>List of assistants</returns>
    [HttpGet]
    public IActionResult GetAssistants()
    {
        // Placeholder for listing assistants
        return Ok(new { Message = "Assistant API is running", Timestamp = DateTime.UtcNow });
    }
}