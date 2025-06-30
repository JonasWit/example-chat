using System.Text;

namespace chat.service.Services.Utilities;

public class LoremIpsumGenerator(int? seed = null)
{
    private static readonly string[] LoremWords =
    [
        "lorem", "ipsum", "dolor", "sit", "amet", "consectetur",
        "adipiscing", "elit", "sed", "do", "eiusmod", "tempor",
        "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua", "ai", "slop"
    ];

    private readonly Random _random = seed.HasValue ? new Random(seed.Value) : new Random();

    private string GenerateSentence(int minWords = 3, int maxWords = 12)
    {
        var wordCount = _random.Next(minWords, maxWords + 1);
        var sentence = new StringBuilder();

        for (var i = 0; i < wordCount; i++)
        {
            var word = LoremWords[_random.Next(LoremWords.Length)];
            if (i == 0) word = char.ToUpper(word[0]) + word[1..];

            sentence.Append(word);

            if (i < wordCount - 1)
                sentence.Append(' ');
        }

        sentence.Append('.');
        return sentence.ToString();
    }

    private string GenerateParagraph(int minSentences = 1, int maxSentences = 7)
    {
        var sentenceCount = _random.Next(minSentences, maxSentences + 1);
        var paragraph = new StringBuilder();

        for (var i = 0; i < sentenceCount; i++)
        {
            paragraph.Append(GenerateSentence());
            if (i < sentenceCount - 1)
                paragraph.Append(' ');
        }

        return paragraph.ToString();
    }

    public string GenerateText(int minParagraphs = 1, int maxParagraphs = 3)
    {
        var paragraphCount = _random.Next(minParagraphs, maxParagraphs + 1);
        var text = new StringBuilder();

        for (var i = 0; i < paragraphCount; i++)
        {
            text.AppendLine(GenerateParagraph());
            if (i < paragraphCount - 1) text.AppendLine();
        }

        return text.ToString().Trim();
    }
}