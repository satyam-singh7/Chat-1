using Chat.Data;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private readonly ApplicationDbContext _context;
    private static Dictionary<string, string> _users = new();

    public ChatHub(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task SendMessage(string groupName, string user, string message, string mediaUrl)
    {
        try
        {
            var chatMessage = new ChatMessage
            {
                GroupName = groupName,
                Sender = user,
                Message = message,
                MediaUrl = mediaUrl ?? "", 
                Timestamp = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message, mediaUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving message: {ex.Message}");
            throw; 
        }
    }



   public async Task JoinGroup(string groupName, string user)
{
    try
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _users[Context.ConnectionId] = user;

        await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"{user} joined the group.", "system");

        // DO NOT SEND PREVIOUS MESSAGES HERE
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error joining group: {ex.Message}");
    }
}


    public async Task LeaveGroup(string groupName, string user)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _users.Remove(Context.ConnectionId);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"{user} left the group.", "system");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error leaving group: {ex.Message}");
        }
    }

  
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_users.TryGetValue(Context.ConnectionId, out var user))
        {
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{user} has disconnected.", "system");
            _users.Remove(Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
