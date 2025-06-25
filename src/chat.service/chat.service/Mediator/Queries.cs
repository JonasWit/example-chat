using chat.service.Data.Entities;
using chat.service.Data.Repositories;
using MediatR;

namespace chat.service.Mediator;

public record GetAllMessagesQuery() : IRequest< List<ChatMessage>>;

public class GetTimeQueryHandler(ChatRepository chatRepository)
    : IRequestHandler<GetAllMessagesQuery, List<ChatMessage>>
{
    public async Task<List<ChatMessage>> Handle(GetAllMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await chatRepository.GetOrderedMessages(cancellationToken);
        return messages;
    }
}