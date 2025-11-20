using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabAI_API.Domain.Entites
{
    public class Word
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int LevelId { get; set; }
        public Level Level { get; set; }
    }
}
