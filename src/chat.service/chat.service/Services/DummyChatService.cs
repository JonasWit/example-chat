using System.Runtime.CompilerServices;
using chat.service.Services.Utilities;

namespace chat.service.Services;

public class DummyChatService : IDummyChatService
{
    public async IAsyncEnumerable<string> GenerateResponse(string prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var generator = new LoremIpsumGenerator();
        var result = generator.GenerateText(1, 2);
        var parts = result.Split(' ');
        foreach (var part in parts)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(200);
            yield return $" {part} ";
        }
    }
}