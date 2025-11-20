using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabAI_API.Domain.Entites
{
    public class Level
    {
        public Level()
        {
            Words = new List<Word>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Word> Words { get; set; }
    }
}
