using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabAI_API.Persistence.Services.Abstraction.UnitOfWork
{
    public interface IUnitOfWork
    {
        public Task SaveChanges();
    }
}
