using Microsoft.EntityFrameworkCore;
using URLShortener.Context;

namespace URLShortener.Services;

public class UrlShorteningService
{
    public const int NumberOfCharsInShortLink = 7;
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private readonly Random random = new Random();
    private readonly ContextBase contextBase;

    public UrlShorteningService(ContextBase contextBase)
    {
        this.contextBase = contextBase;
    }

    public async Task<string> GenerateShortUrlCode()
    {
        var codeChars = new char[NumberOfCharsInShortLink];
        while (true)
        {
            for (int i = 0; i < NumberOfCharsInShortLink; i++)
            {
                var randomIndex = random.Next(Alphabet.Length - 1);

                codeChars[i] = Alphabet[randomIndex];
            }

            string code = new string(codeChars);

            if (!await contextBase.ShortenedUrls.AnyAsync(x => x.Code == code))
            {
                return code;
            }
        }
    }

}
