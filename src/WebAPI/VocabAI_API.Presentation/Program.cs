using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using VocabAI_API.Application.Dtos.Words;
using VocabAI_API.Application.Wrappers.Requests.Word;
using VocabAI_API.Persistence.Context;
using VocabAI_API.Persistence.Services.Abstraction.UnitOfWork;
using VocabAI_API.Persistence.Services.Abstraction.Words;
using VocabAI_API.Persistence.Services.Concrete.UnitOfWork;
using VocabAI_API.Persistence.Services.Concrete.Words;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<VocabAIDBContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("VocabAI_DB")));
builder.Services.AddHttpClient();
builder.Services.AddScoped<IWordService, WordService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.MapPost("/getAIDescription", async (IHttpClientFactory httpClientFactory, IConfiguration config, GetAIDefinitionRequest request) =>
//{
//    if (string.IsNullOrEmpty(request.Word) || string.IsNullOrEmpty(request.Language))
//    {
//        return Results.BadRequest(new { Message = "Kelime ve Dil alanlarý zorunludur." });
//    }

//    string prompt = $$"""
//        **INSTRUCTIONS:** 
//        1. The input word is '{{request.Word}}'.
//        2. Analyze the 'corrected_word' and determine its proficiency level (e.g., A1, B2, C2, or General/Advanced).
//        3. Generate a complete JSON object based on the required JSON schema below.
//        4. The 'corrected_word' field MUST be the most logical, English spelling of the input word.
//        5. The 'definition' and 'explanation' fields MUST be exclusively in the **{{request.Language}}** language.
//        6. The 'example' field MUST be a single, illustrative sentence in **ENGLISH**.
//        7. The 'explanation' field MUST ONLY be an analysis of the 'example' sentence, showing how the corrected word is used in that specific context to illustrate its meaning. DO NOT include general information or spelling correction details.
//        **REQUIRED JSON SCHEMA:**
//        {
//        "corrected_word": "string",
//        "word_level": "string",
//        "definition": "string",
//        "example": "string",
//        "explanation": "string"
//        }
//        """;
//    var apiRequest = new
//    {
//        contents = new[]
//        {
//            new { parts = new[] { new { text = prompt } } }
//        },
//        generationConfig = new
//        {
//            temperature = 0.7
//        }
//    };

//    var jsonPayload = JsonSerializer.Serialize(apiRequest);
//    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

//    string apiUrl = $"{config["GEMINI_API:URL"]}{config["GEMINI_API:KEY"]}";

//    using (var httpClient = new HttpClient())
//    {
//        try
//        {
//            var response = await httpClient.PostAsync(apiUrl, content);
//            if (!response.IsSuccessStatusCode)
//            {
//                string errorContent = await response.Content.ReadAsStringAsync();
//                throw new HttpRequestException($"API isteði baþarýsýz oldu: {response.StatusCode}. Detay: {errorContent}");
//            }
//            response.EnsureSuccessStatusCode();
            
//            var responseBody = await response.Content.ReadAsStringAsync();
//            var apiResponse = JsonDocument.Parse(responseBody);

//            string generatedText = apiResponse
//                .RootElement.GetProperty("candidates")[0]
//                .GetProperty("content").GetProperty("parts")[0]
//                .GetProperty("text").GetString();

//            if (string.IsNullOrWhiteSpace(generatedText))
//            {
//                return Results.StatusCode(500);
//            }

//            string cleanJsonString = generatedText.Trim();

//            if (cleanJsonString.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
//                cleanJsonString = cleanJsonString.Substring("```json".Length).Trim();

//            if (cleanJsonString.EndsWith("```"))
//                cleanJsonString = cleanJsonString.Substring(0, cleanJsonString.Length - "```".Length).Trim();

//            cleanJsonString = cleanJsonString
//                .Replace("\\\"", "\"")
//                .Replace("\\n", "")
//                .Replace("\n", "");

//            if (cleanJsonString.StartsWith("{") && cleanJsonString.EndsWith("}"))
//            {
//                GetAIDefinitionDto? finalData = JsonSerializer.Deserialize<GetAIDefinitionDto>(cleanJsonString);
//                finalData.IsCorrected = !finalData.CorrectedWord.Equals(request.Word, StringComparison.OrdinalIgnoreCase);
//                return Results.Ok(finalData);
//            }
//            else
//            {
//                return Results.StatusCode(500);
//            }
//        }
//        catch (Exception ex)
//        {
//            return Results.StatusCode(500);
//        }
//    }
//});

app.Run();
