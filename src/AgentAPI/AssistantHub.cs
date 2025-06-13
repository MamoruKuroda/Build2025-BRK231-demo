// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license.

using Microsoft.AspNetCore.SignalR;

namespace MyOpenAIWebApi.Hubs;

/// <summary>
/// SignalR hub for real-time assistant communication
/// </summary>
public class AssistantHub : Hub
{
    /// <summary>
    /// Sends a message to all connected clients
    /// </summary>
    /// <param name="user">The user sending the message</param>
    /// <param name="message">The message content</param>
    /// <returns>Task representing the async operation</returns>
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    /// <summary>
    /// Joins a specific group
    /// </summary>
    /// <param name="groupName">The group to join</param>
    /// <returns>Task representing the async operation</returns>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Leaves a specific group
    /// </summary>
    /// <param name="groupName">The group to leave</param>
    /// <returns>Task representing the async operation</returns>
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}