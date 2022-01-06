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
    public class ProfessorRepository : RepositoryBase<Guid, Professor>, IProfessorRepository
    {
        public ProfessorRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override Professor BuscarPeloId(Guid id)
        {
            return _connection.QuerySingle<Professor>(
                @"SELECT * 
                    FROM MATERIA 
                   WHERE ID = :Id", new { id });
        }

        

    }

}