using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabAI_API.Application.Wrappers.Requests
{
    public record GetAIDescriptionRequest(string Word, string Language);
}
