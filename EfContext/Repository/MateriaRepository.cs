

using Dominio;
using Dominio.Entities;
using EfContext.Repository;
using System;

namespace EfContext
{
    public class MateriaRepository : RepositoryBase<Guid, Materia>, IMateriaRepositoryEfCore
    {
        public MateriaRepository(AppContext context) : base(context)
        {

        }
    }
}


