using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Donutella.Application.Services;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly IProductService _products;

    public GeminiService(
        HttpClient http,
        IConfiguration config,
        IProductService products)
    {
        _http = http;
        _config = config;
        _products = products;
    }

    public async Task<string> AskAsync(
        string message,
        IEnumerable<(string Role, string Text)>? history = null)
    {
        var apiKey = _config["Gemini:ApiKey"]
            ?? throw new InvalidOperationException("Gemini API key not configured.");

        var model = _config["Gemini:Model"] ?? "gemini-2.0-flash";

        var catalog = await BuildCatalogAsync();

        var contents = new List<object>();

        if (history != null)
        {
            foreach (var (role, content) in history.TakeLast(8))
            {
                if (string.IsNullOrWhiteSpace(content))
                    continue;

                contents.Add(new
                {
                    role = role == "model" ? "model" : "user",
                    parts = new[] { new { text = content.Trim() } }
                });
            }
        }

        contents.Add(new
        {
            role = "user",
            parts = new[] { new { text = message.Trim() } }
        });

        var body = new
        {
            system_instruction = new
            {
                parts = new[]
                {
                    new
                    {
                        text = "You are Donutella Assistant, a friendly helper for " +
                               "Donutella online dessert store (donuts, ice cream, " +
                               "cupcakes, cookies). When the customer asks about " +
                               "products, categories, prices or availability, you MUST " +
                               "answer using ONLY the Live catalog below: mention the " +
                               "real product names with their prices, and say which " +
                               "ones are out of stock. Never invent products. " +
                               "PRIVACY RULE: you have no access to any user, account, " +
                               "order or personal data. Never reveal, guess or discuss " +
                               "emails, phones, addresses, passwords or other customers' " +
                               "info. If asked for such data, refuse politely and offer " +
                               "store help instead. " +
                               "Also explain how to shop: browse Store, add to cart " +
                               "(login required), checkout with name/address/phone, " +
                               "track orders in My Orders, save favorites in Wishlist. " +
                               "Keep replies short (under 80 words) but always complete " +
                               "sentences. If asked about unrelated topics, " +
                               "politely steer back to the store.\n\nLive catalog:\n" + catalog
                    }
                }
            },
            contents,
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 2048
            }
        };

        using var response = await _http.PostAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}",
            new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"));

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        var text = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return string.IsNullOrWhiteSpace(text)
            ? "Sorry, I could not come up with an answer. Please try again."
            : text.Trim();
    }

    private async Task<string> BuildCatalogAsync()
    {
        try
        {
            var products = await _products.GetAllAsync();

            var lines = products.Select(p =>
                $"- {p.Name}: {p.Price:C} | " +
                $"{(p.Stock > 0 ? $"in stock ({p.Stock})" : "out of stock")}");

            var catalog = string.Join("\n", lines);

            return string.IsNullOrWhiteSpace(catalog)
                ? "(catalog is empty right now)"
                : catalog;
        }
        catch
        {
            return "(catalog unavailable)";
        }
    }
}
