using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BookSphere.Hubs;

public class BookHubs : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Log connection
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = "Connected to BookSphere real-time notifications",
            timestamp = DateTime.UtcNow
        });
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Log disconnection
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = "Disconnected from BookSphere real-time notifications",
            timestamp = DateTime.UtcNow
        });
        
        await base.OnDisconnectedAsync(exception);
    }
    
    // Method for clients to join specific notification groups
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = $"Joined group: {groupName}",
            timestamp = DateTime.UtcNow
        });
    }
    
    // Method for clients to leave specific notification groups
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = $"Left group: {groupName}",
            timestamp = DateTime.UtcNow
        });
    }
    
    // Method to subscribe to specific user's notifications
    public async Task SubscribeToUser(int userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = $"Subscribed to user {userId} notifications",
            timestamp = DateTime.UtcNow
        });
    }
    
    // Method to unsubscribe from specific user's notifications
    public async Task UnsubscribeFromUser(int userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = $"Unsubscribed from user {userId} notifications",
            timestamp = DateTime.UtcNow
        });
    }
    
    // Method to subscribe to book notifications
    public async Task SubscribeToBook(int bookId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"book_{bookId}");
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = $"Subscribed to book {bookId} notifications",
            timestamp = DateTime.UtcNow
        });
    }
    
    // Method to unsubscribe from book notifications
    public async Task UnsubscribeFromBook(int bookId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"book_{bookId}");
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = $"Unsubscribed from book {bookId} notifications",
            timestamp = DateTime.UtcNow
        });
    }
    
    // Method to subscribe to all notifications
    public async Task SubscribeToAll()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "all");
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = "Subscribed to all notifications",
            timestamp = DateTime.UtcNow
        });
    }
    
    // Method to unsubscribe from all notifications
    public async Task UnsubscribeFromAll()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "all");
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = "Unsubscribed from all notifications",
            timestamp = DateTime.UtcNow
        });
    }
    
    // Method to get current connection status
    public async Task GetConnectionStatus()
    {
        await Clients.Caller.SendAsync("ReceiveNotification", new
        {
            type = "system",
            message = "Connection status check",
            connectionId = Context.ConnectionId,
            timestamp = DateTime.UtcNow
        });
    }
}
