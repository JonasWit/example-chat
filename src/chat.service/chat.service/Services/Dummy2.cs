using System.Runtime.CompilerServices;
using chat.service.Services.Utilities;

namespace chat.service.Services;

public class Dummy2 : IDummyChatService
{
    public async IAsyncEnumerable<string> GenerateResponse(string prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var generator = new LoremIpsumGenerator(123);
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