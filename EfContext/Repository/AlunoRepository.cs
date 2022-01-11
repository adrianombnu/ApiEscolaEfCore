using Dominio;
using System;

namespace EfContext.Repository
{
    public class AlunoRepository : RepositoryBase<Guid, Dominio.Entities.Aluno>, IAlunoRepositoryEfCore
    {
        public AlunoRepository(AppContext context) : base(context)
        {

        }
    }
}


