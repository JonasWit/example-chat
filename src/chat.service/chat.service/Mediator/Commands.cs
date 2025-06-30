using chat.service.Data;
using chat.service.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace chat.service.Mediator;

public record CreateChatMessageCommand(ChatMessage NewMessage) : IRequest<long>;

public class CreateChatMessageHandler(IDbContextFactory<ChatContext> contextFactory)
    : IRequestHandler<CreateChatMessageCommand, long>
{
    public async Task<long> Handle(CreateChatMessageCommand request, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await context.AddAsync(request.NewMessage, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return request.NewMessage.Id;
    }
}

public record RateChatMessageCommand(long Id, MessageScore Score) : IRequest;

public class RateChatMessageHandler(IDbContextFactory<ChatContext> contextFactory)
    : IRequestHandler<RateChatMessageCommand>
{
    public async Task Handle(RateChatMessageCommand request, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await context.ChatMessages
            .Where(u => u.Id == request.Id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(u => u.Score, request.Score), cancellationToken);
    }
}