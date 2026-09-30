namespace Donutella.Application.Services;

public interface IGeminiService
{
    Task<string> AskAsync(
        string message,
        IEnumerable<(string Role, string Text)>? history = null);
}
