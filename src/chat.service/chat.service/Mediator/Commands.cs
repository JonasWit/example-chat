using chat.service.Data.Entities;
using chat.service.Data.Repositories;
using MediatR;

namespace chat.service.Mediator;

public record CreateChatMessageCommand(ChatMessage NewMessage) : IRequest<long>;

public class CreateChatMessageHandler(ChatRepository chatRepository) : IRequestHandler<CreateChatMessageCommand, long>
{
    public async Task<long> Handle(CreateChatMessageCommand request, CancellationToken cancellationToken)
    {
        await chatRepository.SaveMessage(request.NewMessage);
        await chatRepository.SaveChanges(cancellationToken);
        return request.NewMessage.Id;
    }
}

public record UpdateChatMessageTextCommand(long Id, string Text) : IRequest;

public class UpdateChatMessageTextHandler(ChatRepository chatRepository) : IRequestHandler<UpdateChatMessageTextCommand>
{
    public async Task Handle(UpdateChatMessageTextCommand request, CancellationToken cancellationToken)
    {
        await chatRepository.ChangeMessageText(request.Id, request.Text);
    }
}

public record RateChatMessageCommand(long Id, int Score) : IRequest;

public class RateChatMessageHandler(ChatRepository chatRepository) : IRequestHandler<RateChatMessageCommand>
{
    public async Task Handle(RateChatMessageCommand request, CancellationToken cancellationToken)
    {
        await chatRepository.ChangeMessageScore(request.Id, request.Score);
    }
}