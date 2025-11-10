using AppChat.Data;
using AppChat.Models;

namespace AppChat.Services
{
    public class MessageService
    {
        private readonly AppDbContext _context;

        public MessageService(AppDbContext context) => _context = context;

        public async Task<Message> SaveMessage(string converId, string senderId, string content)
        {
            var msg = new Message
            {
                ConversationId = int.Parse(converId),
                SenderId = int.Parse(senderId),
                Content = content,
                SentAt = DateTime.UtcNow,
            };

            await _context.Messages.AddAsync(msg);
            await _context.SaveChangesAsync();

            return msg;
        }

        //public async Task MarkAsSeenAsync(int messageId)
        //{
        //    var msg = await _context.Messages.FindAsync(messageId);
        //    if (msg != null)
        //    {
        //        msg.Status = "Seen";
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}
