using chat.service.Data;
using chat.service.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace chat.service.Mediator;

public record ChatMessageProjection(long Id, string SentBy, string Text, string Score);
    
public record GetAllMessagesQuery : IRequest<List<ChatMessageProjection>>;

public class GetTimeQueryHandler(IDbContextFactory<ChatContext> contextFactory)
    : IRequestHandler<GetAllMessagesQuery, List<ChatMessageProjection>>
{
    public async Task<List<ChatMessageProjection>> Handle(GetAllMessagesQuery request, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.ChatMessages
            .OrderBy(cm => cm.Id)
            .Select(m => new ChatMessageProjection(m.Id, m.SentBy, m.Text, m.Score.ToString()))
            .ToListAsync(cancellationToken);
    }
}