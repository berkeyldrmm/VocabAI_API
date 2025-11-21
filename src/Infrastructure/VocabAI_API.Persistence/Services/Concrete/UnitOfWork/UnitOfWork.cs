using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabAI_API.Persistence.Context;
using VocabAI_API.Persistence.Services.Abstraction.UnitOfWork;

namespace VocabAI_API.Persistence.Services.Concrete.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VocabAIDBContext _context;

        public UnitOfWork(VocabAIDBContext context)
        {
            _context = context;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
