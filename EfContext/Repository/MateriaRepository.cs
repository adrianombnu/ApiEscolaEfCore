using EfContext.Entities;
using System;

namespace EfContext.Repository
{
    public class MateriaRepository : RepositoryBase<Materium>, IMateriaRepositoryEfCore
    {
        public MateriaRepository(ModelContext context) : base(context)
        {

        }

       
    }
}


