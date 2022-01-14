using EfContext.Entities;
using System;

namespace EfContext.Repository
{
    public class AlunoRepository : RepositoryBase<Aluno>, IAlunoRepositoryEfCore
    {
        public AlunoRepository(ModelContext context) : base(context)
        {

        }
    }
}


