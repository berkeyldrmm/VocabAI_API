using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using VocabAI_API.Application.Wrappers.Requests.Word;
using VocabAI_API.Domain.Entites;
using VocabAI_API.Persistence.Services.Abstraction.Words;

namespace VocabAI_API.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWordService _wordService;
        public WordController(IConfiguration config, IWordService wordService)
        {
            _config = config;
            _wordService = wordService;
        }

        [HttpPost("/getaidefinition")]
        public async Task<IActionResult> GetAIDefinition(GetAIDefinitionRequest request)
        {
            if (string.IsNullOrEmpty(request.Word) || string.IsNullOrEmpty(request.Language))
            {
                return BadRequest(new { Message = "Kelime ve Dil alanları zorunludur." });
            }

            try
            {
                var dto = await _wordService.GetAIDefinition(request.Word, request.Language);
                if(!await _wordService.CheckWordExists(dto.CorrectedWord))
                {
                    await _wordService.Add(new Word()
                    {
                        Id = Guid.NewGuid(),
                        Name = dto.CorrectedWord,
                        LevelId = dto.WordLevel.ToUpper() switch
                        {
                            "A1" => 1,
                            "A2" => 2,
                            "B1" => 3,
                            "B2" => 4,
                            "C1" => 5,
                            "C2" => 6,
                            _ => 0,
                        }
                    });
                }
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("/getwordsbylevel/{levelId}")]
        public async Task<IActionResult> GetWordsByLevel(int levelId)
        {
            try
            {
                var words = await _wordService.GetByLevel(levelId);
                return Ok(words);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
