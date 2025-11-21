using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VocabAI_API.Application.Dtos.Words
{
    public record GetAIDefinitionDto(
        [property: JsonPropertyName("corrected_word")] string CorrectedWord,
        [property: JsonPropertyName("word_level")] string WordLevel,
        [property: JsonPropertyName("definition")] string Definition,
        [property: JsonPropertyName("example")] string Example,
        [property: JsonPropertyName("explanation")] string Explanation
    )
    {
        [JsonPropertyName("isCorrected")]
        public bool IsCorrected { get; set; }
    }
}
