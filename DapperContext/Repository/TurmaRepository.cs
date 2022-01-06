using Dapper;
using Dominio;
using Dominio.Entities;
using Dominio.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperContext.Repository
{
    public class TurmaRepository : RepositoryBase<Guid, Turma>, ITurmaRepository
    {
        public TurmaRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override Turma BuscarPeloId(Guid id)
        {
            return _connection.QuerySingle<Turma>(
                @"SELECT * 
                    FROM MATERIA 
                   WHERE ID = :Id", new { id });
        }

        

    }

}