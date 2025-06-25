using chat.service.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace chat.service.Data.Repositories;

public class ChatRepository(ChatContext context) : RepositoryBase<ChatContext>(context)
{
    public ValueTask<EntityEntry<ChatMessage>> SaveMessage(ChatMessage message) => _context.AddAsync(message);
    public Task<List<ChatMessage>> GetOrderedMessages(CancellationToken cancellationToken) => 
        _context.ChatMessages.OrderBy(cm => cm.Id).ToListAsync(cancellationToken);
    
    public async Task ChangeMessageText(long id, string text)
    {
        await _context.ChatMessages
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(setters => 
                setters.SetProperty(u => u.Text, text));
    }
    
    public async Task ChangeMessageScore(long id, int score)
    {
        await _context.ChatMessages
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(setters => 
                setters.SetProperty(u => u.Score, (MessageScore)score));
    }
}