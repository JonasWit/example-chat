using chat.service.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace chat.service.Data;

public class ChatContext :  DbContext
{
    public DbSet<ChatMessage> ChatMessages { get; set; }
    
    public ChatContext(DbContextOptions<ChatContext> options) : base(options)
    {
    }
}