namespace chat.service.Data.Entities;

public record ChatMessage
{
    public long Id { get; set; }
    public string SentBy { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public MessageScore Score { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}