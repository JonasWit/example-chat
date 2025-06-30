using System.Text;
using System.Text.Json;
using chat.service.Data.Entities;
using chat.service.Data.Repositories;
using chat.service.Endpoints.Contracts;
using chat.service.Mediator;
using chat.service.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace chat.service.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("ai-chat");
        group.MapGet("/all-messages", GetMessages).WithDisplayName(nameof(GetMessages));
        group.MapPost("/new-message", NewMessage).WithDisplayName(nameof(NewMessage));
        group.MapPut("/rate-message", RateMessage).WithDisplayName(nameof(RateMessage));
    }

    private static async Task<IResult> RateMessage(
        [FromBody] ScoreChangeRequest request,
        [FromServices] IMediator mediator,
        [FromServices] ChatRepository chatRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new RateChatMessageCommand(request.Id, request.Score), cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return Results.Ok("Score changed");
    }

    private static async Task<IResult> GetMessages(
        [FromServices] IMediator mediator,
        [FromServices] ChatRepository chatRepository, 
        CancellationToken cancellationToken)
    {
        var messages =  await mediator.Send(new GetAllMessagesQuery(), cancellationToken);
        return Results.Ok(messages);
    }

    private static async Task NewMessage(
        HttpContext context,
        [FromBody] IncomingMessage message,
        [FromServices] IMediator mediator,
        [FromServices] ChatProvider chatProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            IDummyChatService? currentChatService = null;
            if (DateTime.UtcNow.DayOfWeek != DayOfWeek.Monday)
            {
                currentChatService = chatProvider.GetService(ChosenService.S1);
            }
            else
            {
                currentChatService = chatProvider.GetService(ChosenService.S2);     
            }

            if (currentChatService is null)
            {
                throw new NotImplementedException();
            }
            
            var prompt = new ChatMessage
            {
                SentBy = "Person",
                Text = message.Text
            };
            var botMessage = new ChatMessage
            {
                SentBy = "Bot",
                Text = ""
            };
            prompt.Id = await mediator.Send(new CreateChatMessageCommand(prompt), cancellationToken);
            botMessage.Id = await mediator.Send(new CreateChatMessageCommand(botMessage), cancellationToken);
            
            cancellationToken.ThrowIfCancellationRequested();
            
            context.Response.Headers.Add("Cache-Control", "no-cache");
            context.Response.Headers.Add("Content-Type", "application/x-ndjson");
            context.Response.Headers.Add("Connection", "keep-alive");
            
            var promptJson = JsonSerializer.Serialize(new
                { id = prompt.Id, sentBy = prompt.SentBy, text = prompt.Text, score = prompt.Score });
            var botMessageJson = JsonSerializer.Serialize(new
                { id = botMessage.Id, sentBy = botMessage.SentBy, text = botMessage.Text, score = botMessage.Score });
            await context.Response.WriteAsync(promptJson + "\n");
            await context.Response.Body.FlushAsync(cancellationToken);
            await context.Response.WriteAsync(botMessageJson + "\n");
            await context.Response.Body.FlushAsync(cancellationToken);

            var botMessageString = new StringBuilder();
            await foreach (var chunk in currentChatService.GenerateResponse(message.Text, cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    await mediator.Send(new UpdateChatMessageTextCommand(botMessage.Id, botMessageString.ToString()), cancellationToken);
                    await context.Response.CompleteAsync();
                    context.Abort();
                    return;
                }

                var json = JsonSerializer.Serialize(new { data = chunk });
                await context.Response.WriteAsync(json + "\n");
                await context.Response.Body.FlushAsync();

                botMessageString.Append(chunk);
                botMessageString.Append(' ');
            }

            await mediator.Send(new UpdateChatMessageTextCommand(botMessage.Id, botMessageString.ToString().Trim()), cancellationToken);
            await context.Response.CompleteAsync();
            context.Abort();
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine("Operation cancelled");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            await context.Response.CompleteAsync();
            context.Abort();          
        }
    }
}