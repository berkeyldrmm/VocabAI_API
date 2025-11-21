using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabAI_API.Application.Dtos.Words
{
    public class GetWordsByLevelDto
    {
        public string WordName { get; set; }
        public string WordLevel { get; set; }
    }
}
