using System.Text;
using chat.service.Data.Entities;
using chat.service.Hubs;
using chat.service.Mediator;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace chat.service.Services;

public class ChatBotStreamingService(
    IHubContext<BotGeneratorHub, IChatHub> chatHubContext,
    IMediator mediator,
    ChatProvider chatProvider)
{
    private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = [];
    public void CancelStreamForUser(string userId)
    {
        if (_cancellationTokens.TryGetValue(userId, out var cts)) cts.Cancel();
    }

    public Task StartResponseStreamForUser(string userId, string prompt)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var newCts = new CancellationTokenSource();
                if (_cancellationTokens.TryGetValue(userId, out var cts))
                {
                    await cts.CancelAsync();
                    _cancellationTokens.Remove(userId);
                    _cancellationTokens.Add(userId, cts);
                }
                else
                {
                    _cancellationTokens.Add(userId, newCts);         
                }
                
                var botMessage = new ChatMessage
                {
                    SentBy = "Bot",
                    Text = ""
                };
                var botMessageString = new StringBuilder();
                var currentChatService =
                    chatProvider.GetService(DateTime.UtcNow.DayOfWeek != DayOfWeek.Monday
                        ? ChosenService.S1
                        : ChosenService.S2);
                
                await foreach (var chunk in currentChatService.GenerateResponse(prompt, newCts.Token))
                {
                    botMessageString.Append(chunk);
                    await chatHubContext.Clients
                        .Groups("dummy-user")
                        .ResponseGenerated(new ChatBotResponseMessage(chunk));
                    if (!newCts.Token.IsCancellationRequested) continue;

                    botMessage.Text = botMessageString.ToString();
                    botMessage.Id = await mediator.Send(new CreateChatMessageCommand(botMessage));
                    await chatHubContext.Clients
                        .Groups("dummy-user")
                        .StreamCompleted(new StreamCompletedMessage(botMessage.Id, botMessageString.ToString()));
                    return;
                }

                botMessage.Text = botMessageString.ToString();
                botMessage.Id = await mediator.Send(new CreateChatMessageCommand(botMessage));
                await chatHubContext.Clients
                    .Groups("dummy-user")
                    .StreamCompleted(new StreamCompletedMessage(botMessage.Id, botMessageString.ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in stream task: {ex.Message}");
            }
            finally
            {
                _cancellationTokens.Remove(userId);
            }
        });
        return Task.CompletedTask;
    }
}