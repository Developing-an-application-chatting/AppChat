using AppChat.Data;
using AppChat.Services;
using Microsoft.AspNetCore.SignalR;

namespace AppChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly MessageService _msgService;

        public ChatHub(AppDbContext context, MessageService service)
        {
            _context = context;
            _msgService = service;
        }

        public string UserId => Context.UserIdentifier ?? "NULL";

        public override Task OnConnectedAsync()
        {
            var identity = Context.User?.Identity;
            if (identity != null)
            {
                Console.WriteLine($"IsAuthenticated: {identity.IsAuthenticated}");
                Console.WriteLine($"AuthenticationType: {identity.AuthenticationType}");
                Console.WriteLine($"Name / sub: {Context.User.FindFirst("sub")?.Value}");
            }
            else
            {
                Console.WriteLine("Context.User.Identity IS NULL");
                throw new UnauthorizedAccessException("Unauthorized");
            }

            Console.WriteLine("Claims:");
            if (Context.User != null)
            {
                foreach (var c in Context.User.Claims)
                {
                    Console.WriteLine($"  {c.Type}: {c.Value}");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }
            return base.OnConnectedAsync();
        }

        public async Task JoinGroup(string chatId)
        {
            Console.WriteLine($"[JoinGroup] called chatId = '{chatId}' on ConnectionId = {Context.ConnectionId}");
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
                Console.WriteLine("Joined group successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in JoinGroup: " + ex);
                throw new HubException("JoinGroup error: " + ex.Message);
            }
        }

    }
}
