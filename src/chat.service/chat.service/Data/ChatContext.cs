using chat.service.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace chat.service.Data;

public class ChatContext : DbContext
{
    public ChatContext(DbContextOptions<ChatContext> options) : base(options)
    {
    }

    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<ChatMessage>()
            .Property(e => e.Score)
            .HasConversion<string>();
        base.OnModelCreating(modelBuilder);
    }
}