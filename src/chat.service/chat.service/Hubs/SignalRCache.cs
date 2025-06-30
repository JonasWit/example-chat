namespace chat.service.Hubs;

public class SignalRCache(ILogger<SignalRCache> logger)
{
    private readonly SemaphoreSlim _signalRChatSemaphore = new(1);
    private Dictionary<string, HashSet<string>> ConnectedChatUsers { get; } = new();

    public async Task AddChatUser(string userId, string connectionId)
    {
        try
        {
            await _signalRChatSemaphore.WaitAsync(); // can be semaphore of concurrent dictionary
            if (ConnectedChatUsers.TryGetValue(userId, out var connections))
                _ = connections.Add(connectionId);
            else
                ConnectedChatUsers.Add(userId, [connectionId]);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddChatUser exception");
        }
        finally
        {
            _ = _signalRChatSemaphore.Release();
        }
    }

    public async Task RemoveChatUserConnection(string userId, string connectionId)
    {
        try
        {
            await _signalRChatSemaphore.WaitAsync();
            if (ConnectedChatUsers.TryGetValue(userId, out var connections)) _ = connections.Remove(connectionId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "RemoveChatUserConnection exception");
        }
        finally
        {
            _ = _signalRChatSemaphore.Release();
        }
    }
}