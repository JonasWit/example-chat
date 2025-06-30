using Microsoft.AspNetCore.SignalR;

namespace chat.service.Hubs;

public record ChatBotResponseMessage(string Text);

public record StreamCompletedMessage(long BotMessageId, string FullMessage);

public interface IChatHub
{
    Task ResponseGenerated(ChatBotResponseMessage message);
    Task StreamCompleted(StreamCompletedMessage message);
}

public class BotGeneratorHub(
    SignalRCache signalRConnectionsMonitor) : Hub<IChatHub>
{
    public override async Task OnConnectedAsync()
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,
                "dummy-user"); // this dummy-user would be a real userId from database
            await signalRConnectionsMonitor.AddChatUser("dummy-user",
                Context.ConnectionId); // this way we can track all connections for a user
            _ = base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            await signalRConnectionsMonitor.RemoveChatUserConnection("dummy-user", Context.ConnectionId);
            _ = base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
    }
}