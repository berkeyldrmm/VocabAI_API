using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using VocabAI_API.Application.Dtos;
using VocabAI_API.Application.Wrappers.Requests;
using VocabAI_API.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<VocabAIDBContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("VocabAI_DB")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPost("/getAIDescription", async (VocabAIDBContext conetxt, IConfiguration config, GetAIDescriptionRequest request) =>
{
    if (string.IsNullOrEmpty(request.Word) || string.IsNullOrEmpty(request.Language))
    {
        return Results.BadRequest(new { Message = "Kelime ve Dil alanlarý zorunludur." });
    }

    string prompt = $$"""
        **INSTRUCTIONS:** 
        1. The input word is '{{request.Word}}'.
        2. If the word appears to be misspelled or uncommon, find the most logical, commonly known English spelling/word. If the word is correct, use the input word. The corrected word MUST be in English.
        3. Generate a complete JSON object based on the required JSON schema below.
        4. Ensure the values for 'definition', 'example', and 'explanation' are exclusively in the **{{request.Language}}** language.
        **REQUIRED JSON SCHEMA:**
        {
        "corrected_word": "string",
        "definition": "string",
        "example": "string",
        "Explanation": "string"
        }
        """;
    var apiRequest = new
    {
        contents = new[]
        {
            new { parts = new[] { new { text = prompt } } }
        },
        generationConfig = new
        {
            temperature = 0.7
        }
    };

    var jsonPayload = JsonSerializer.Serialize(apiRequest);
    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

    string apiUrl = $"{config["GEMINI_API:URL"]}{config["GEMINI_API:KEY"]}";

    using (var httpClient = new HttpClient())
    {
        try
        {
            var response = await httpClient.PostAsync(apiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                // errorContent deðiþkenini Debug/Console'a yazdýrýn
                // Bu metin, hatanýn tam sebebini (API'nýn neden 400 döndürdüðünü) açýklayacaktýr.
                throw new HttpRequestException($"API isteði baþarýsýz oldu: {response.StatusCode}. Detay: {errorContent}");
            }
            response.EnsureSuccessStatusCode();
            
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonDocument.Parse(responseBody);

            string generatedText = apiResponse
                .RootElement.GetProperty("candidates")[0]
                .GetProperty("content").GetProperty("parts")[0]
                .GetProperty("text").GetString();

            string cleanJsonString = generatedText.Replace("\\\"", "\"");

            cleanJsonString = cleanJsonString.Replace("\\n", "").Trim();
            cleanJsonString = cleanJsonString.Replace("\n", "").Trim();

            if (cleanJsonString.StartsWith('"') && cleanJsonString.EndsWith('"'))
            {
                cleanJsonString = cleanJsonString.Trim('"');
            }

            AIResponseDto? finalData = JsonSerializer.Deserialize<AIResponseDto>(generatedText);

            if (finalData == null)
            {
                return Results.StatusCode(500);
            }

            return Results.Ok(new
            {
                CorrectedWord = finalData.CorrectedWord,
                Definition = finalData.Definition,
                Example = finalData.Example,
                Explanation = finalData.Explanation
            });
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }
});

app.Run();
