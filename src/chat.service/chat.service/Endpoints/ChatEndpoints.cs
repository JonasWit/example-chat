using chat.service.Data;
using chat.service.Data.Entities;
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
        group.MapPost("/cancel-message", CancelMessage).WithDisplayName(nameof(CancelMessage));
        group.MapPost("/new-message-signalr", NewMessageWithSignalR).WithDisplayName(nameof(NewMessageWithSignalR));
        group.MapPut("/rate-message", RateMessage).WithDisplayName(nameof(RateMessage));
    }

    private static IResult CancelMessage(
        [FromServices] ChatBotStreamingService chatBotStreamingService,
        CancellationToken cancellationToken)
    {
        try
        {
            chatBotStreamingService.CancelStreamForUser("dummy-user");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return Results.Ok("Score changed");
    }

    private static async Task<IResult> RateMessage(
        [FromBody] ScoreChangeRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            if (Enum.TryParse<MessageScore>(request.Score, true, out var score))
                await mediator.Send(new RateChatMessageCommand(request.Id, score), cancellationToken);
            else
                return Results.BadRequest();
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
        CancellationToken cancellationToken)
    {
        var messages = await mediator.Send(new GetAllMessagesQuery(), cancellationToken);
        return Results.Ok(messages);
    }

    private static async Task<ChatMessage> NewMessageWithSignalR(
        HttpContext context,
        [FromBody] IncomingMessage message,
        [FromServices] IMediator mediator,
        [FromServices] ChatBotStreamingService chatBotStreamingService,
        CancellationToken cancellationToken)
    {
        try
        {
            var usersMessage = new ChatMessage
            {
                SentBy = "Person",
                Text = message.Text
            };
            usersMessage.Id = await mediator.Send(new CreateChatMessageCommand(usersMessage), cancellationToken);
            await chatBotStreamingService.StartResponseStreamForUser("dummy-user", message.Text);
            return usersMessage;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}