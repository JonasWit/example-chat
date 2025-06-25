namespace chat.service.Services;

public interface IDummyChatService
{
    IAsyncEnumerable<string> GenerateResponse(string prompt, CancellationToken ct);
}