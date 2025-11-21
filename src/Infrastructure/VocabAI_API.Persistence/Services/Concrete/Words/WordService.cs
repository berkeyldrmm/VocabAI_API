using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VocabAI_API.Application.Dtos.Words;
using VocabAI_API.Domain.Entites;
using VocabAI_API.Persistence.Context;
using VocabAI_API.Persistence.Services.Abstraction.UnitOfWork;
using VocabAI_API.Persistence.Services.Abstraction.Words;

namespace VocabAI_API.Persistence.Services.Concrete.Words
{
    public class WordService : IWordService
    {
        private readonly VocabAIDBContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        public WordService(VocabAIDBContext context, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task Add(Word word)
        {
            await _context.AddAsync(word);
            await _unitOfWork.SaveChanges();
        }

        public async Task<bool> CheckWordExists(string wordName)
        {
            return await _context.Words.AnyAsync(w => w.Name.ToLower() == wordName.ToLower());
        }

        public async Task<GetAIDefinitionDto> GetAIDefinition(string word, string language)
        {
            string prompt = $$"""
            **INSTRUCTIONS:** 
            1. The input word is '{{word}}'.
            2. Analyze the 'corrected_word' and determine its proficiency level. The level MUST be one of the following exact CEFR codes: A1, A2, B1, B2, C1, or C2. DO NOT use terms like 'General', 'Advanced', or 'Basic'.
            3. Generate a complete JSON object based on the required JSON schema below.
            4. The 'corrected_word' field MUST be the most logical, English spelling of the input word.
            5. The 'definition' and 'explanation' fields MUST be exclusively in the **{{language}}** language.
            6. The 'example' field MUST be a single, illustrative sentence in **ENGLISH**.
            7. The 'explanation' field MUST ONLY be an analysis of the 'example' sentence, showing how the corrected word is used in that specific context to illustrate its meaning. DO NOT include general information or spelling correction details.
            **REQUIRED JSON SCHEMA:**
            {
            "corrected_word": "string",
            "word_level": "string",
            "definition": "string",
            "example": "string",
            "explanation": "string"
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

            string apiUrl = $"{_config["GEMINI_API:URL"]}{_config["GEMINI_API:KEY"]}";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API isteği başarısız oldu: {response.StatusCode}. Detay: {errorContent}");
                }
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonDocument.Parse(responseBody);

                string generatedText = apiResponse
                    .RootElement.GetProperty("candidates")[0]
                    .GetProperty("content").GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                string cleanJsonString = generatedText.Trim();

                if (cleanJsonString.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
                    cleanJsonString = cleanJsonString.Substring("```json".Length).Trim();

                if (cleanJsonString.EndsWith("```"))
                    cleanJsonString = cleanJsonString.Substring(0, cleanJsonString.Length - "```".Length).Trim();

                cleanJsonString = cleanJsonString
                    .Replace("\\\"", "\"")
                    .Replace("\\n", "")
                    .Replace("\n", "");

                GetAIDefinitionDto? finalData = JsonSerializer.Deserialize<GetAIDefinitionDto>(cleanJsonString);
                finalData.IsCorrected = !finalData.CorrectedWord.Equals(word, StringComparison.OrdinalIgnoreCase);
                return finalData;
            }
        }

        public async Task<List<GetWordsByLevelDto>> GetByLevel(int levelId)
        {
            return await _context.Words
                .Where(w => w.LevelId == levelId)
                .Select(w => new GetWordsByLevelDto
                {
                    WordName = w.Name,
                    WordLevel = w.Level.Name
                })
                .ToListAsync();
        }
    }
}
