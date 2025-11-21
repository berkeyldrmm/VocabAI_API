using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabAI_API.Application.Dtos.Words;
using VocabAI_API.Domain.Entites;

namespace VocabAI_API.Persistence.Services.Abstraction.Words
{
    public interface IWordService
    {
        public Task<GetAIDefinitionDto> GetAIDefinition(string word, string language);
        public Task<List<GetWordsByLevelDto>> GetByLevel(int levelId);
        public Task Add(Word word);
        public Task<bool> CheckWordExists(string wordName);
    }
}
