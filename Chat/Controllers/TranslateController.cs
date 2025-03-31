using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[Route("api/translate")]
[ApiController]
public class TranslateController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "AIzaSyB6-c_McEt5IV0orQPOBvdL1UvPjzY1AoA";

    public TranslateController()
    {
        _httpClient = new HttpClient();
    }

    
    [HttpGet("languages")]
    public IActionResult GetLanguages()
    {
        var languages = new List<string> { "French", "Spanish", "German", "Italian", "Chinese", "Japanese" };
        return Ok(languages);
    }


    [HttpGet("text")]
    public async Task<IActionResult> Translate([FromQuery] string text, [FromQuery] string language)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(language))
            return BadRequest("Text and language parameters are required.");

        // Append instruction for accuracy
        text += $" (Translate this strictly into {language} without extra words)";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = $"Translate this to {language}: {text}" }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

        string geminiApiKey = "AIzaSyB6-c_McEt5IV0orQPOBvdL1UvPjzY1AoA"; // Ensure this is correct
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={geminiApiKey}";


        try
        {
            var response = await _httpClient.PostAsync(url, requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            // Debug: Print full response in logs
            Console.WriteLine("API Response: " + responseString);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"API Error: {responseString}");
            }

            // Parse API response safely
            using JsonDocument doc = JsonDocument.Parse(responseString);
            if (!doc.RootElement.TryGetProperty("candidates", out JsonElement candidates) || candidates.GetArrayLength() == 0)
            {
                return BadRequest("Invalid API response structure.");
            }

            var translatedText = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return Ok(new { translatedText });
        }
        catch (HttpRequestException httpEx)
        {
            return StatusCode(500, $"HTTP Request Error: {httpEx.Message}");
        }
        catch (JsonException jsonEx)
        {
            return BadRequest($"JSON Parsing Error: {jsonEx.Message}");
        }
    }

}
