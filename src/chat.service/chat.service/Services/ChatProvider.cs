namespace chat.service.Services;

public enum ChosenService
{
    S1,
    S2
}

public class ChatProvider
{
    public IDummyChatService GetService(ChosenService chosenService)
    {
        switch (chosenService)
        {
            case ChosenService.S1:
                return new DummyChatService();
            case ChosenService.S2:
                return new Dummy2();
            default:
                throw new ArgumentOutOfRangeException(nameof(chosenService), chosenService, null);
        }

        throw new ArgumentOutOfRangeException(nameof(chosenService), chosenService, null);
    }
}