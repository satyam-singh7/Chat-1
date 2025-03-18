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
        text = text + " ....  Strictly give the meniing in hindi nothing else ";
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(language))
            return BadRequest("Text and language parameters are required.");

        // Prepare the request body to send to the Gemini API
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

        // Use the Gemini API key (make sure to replace "YOUR_GEMINI_API_KEY" with your actual API key)
        var geminiApiKey = "AIzaSyB6-c_McEt5IV0orQPOBvdL1UvPjzY1AoA"; // Replace this with your Gemini API key
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={geminiApiKey}";

        // Make the request to Gemini API
        var response = await _httpClient.PostAsync(url, requestContent);
        var responseString = await response.Content.ReadAsStringAsync();


        try
        {
            using JsonDocument doc = JsonDocument.Parse(responseString);
            var translatedText = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return Ok(new { translatedText });
        }
        catch (JsonException ex)
        {
            return BadRequest($"JSON Parsing Error: {ex.Message}");
        }

        //// Parse the response and extract the translated text
        //using JsonDocument doc = JsonDocument.Parse(responseString);
        //string translatedText = doc.RootElement.GetProperty("contents")[0].GetProperty("parts")[0].GetProperty("text").GetString();

        //return Ok(new { translatedText });
    }

}
