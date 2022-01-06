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
    public class CursoRepository : RepositoryBase<Guid, Curso>, ICursoRepository
    {
        public CursoRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override Curso BuscarPeloId(Guid id)
        {
            return _connection.QuerySingle<Curso>(
                @"SELECT * 
                    FROM MATERIA 
                   WHERE ID = :Id", new { id });
        }

        


    }

}