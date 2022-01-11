using Dominio;
using Dominio.Entities;
using System;

namespace EfContext.Repository
{
    public class MateriaRepository : RepositoryBase<Guid, Materia>, IMateriaRepositoryEfCore
    {
        public MateriaRepository(AppContext context) : base(context)
        {

        }

       
    }
}


