using Microsoft.AspNetCore.SignalR;

namespace BookSphere.Hubs;

public class BookHubs : Hub
{
        public async Task BroadcastBookSale(string message, int bookId, string Title)
        {
                await Clients.All.SendAsync("RecieveBookSale", message, bookId, Title);
        }
}
