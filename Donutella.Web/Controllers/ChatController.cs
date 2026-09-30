using Donutella.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Donutella.Web.Controllers;

public class ChatController : Controller
{
    private readonly IGeminiService _gemini;

    public ChatController(IGeminiService gemini)
    {
        _gemini = gemini;
    }

    public class ChatHistoryItem
    {
        public string Role { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public List<ChatHistoryItem>? History { get; set; }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Message))
            return Json(new { reply = "Please type a message first." });

        try
        {
            var history = request.History?
                .Where(h => !string.IsNullOrWhiteSpace(h.Text))
                .Select(h => (h.Role == "bot" ? "model" : "user", h.Text));

            var reply = await _gemini.AskAsync(request.Message, history);
            return Json(new { reply });
        }
        catch
        {
            return Json(new
            {
                reply = "Sorry, the assistant is unavailable right now. Please try again later."
            });
        }
    }
}
