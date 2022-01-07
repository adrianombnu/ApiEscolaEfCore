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
    public class MateriaRepository : RepositoryBase<Guid, Materia>, IMateriaRepository
    {
        public MateriaRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override Materia BuscarPeloId(Guid id)
        {
            return _connection.QuerySingleOrDefault<Materia>(
                @"SELECT * 
                    FROM MATERIA 
                   WHERE ID = :Id", new { id });
        }

        
        public bool VerificaSePossuiTurmaVinculada(Guid idMateria)
        {
            var materia = _connection.QueryFirstOrDefault<Materia>(@"SELECT TM.ID 
                                                               FROM turma_materia tm 
                                                              WHERE tm.idmateria = :IdMateria", new { idMateria });

            if (materia is null)
                return false;
            
            return true;

        }

        public bool VerificaSeMateriaJaCadastrada(string nomeMateria, Guid idMateria, Guid idProfessor, bool consideraIdDifente = false)
        {
            var query = (@"SELECT * FROM MATERIA M WHERE 1 = 1");

            var sb = new StringBuilder(query);

            sb.Append(" AND UPPER(M.NOME) = :NomeMateria ");
            sb.Append(" AND M.IDPROFESSOR = :IdProfessor ");

            if (consideraIdDifente)
                sb.Append(" AND M.ID <> :IdMateria ");

            var materia = _connection.QueryFirstOrDefault<Materia>(sb.ToString(), new { IdMateria = idMateria.ToString(),
                                                                                //NomeMateria = nomeMateria.ToUpperIgnoreNull(),
                                                                                NomeMateria = nomeMateria.ToUpper(),
                                                                                IdProfessor = idProfessor.ToString()
                                                                               });
            if (materia is null)
                return false;

            return true;

        }

        public IEnumerable<Materia> ListarMaterias(string? nome = null, int page = 1, int itens = 50)
        {
            var query = (@"SELECT * FROM (SELECT ROWNUM AS RN, M.* FROM MATERIA M WHERE 1 = 1");

            var sb = new StringBuilder(query);

            if (!string.IsNullOrEmpty(nome))
                sb.Append(" AND UPPER(M.NOME) LIKE '%' || :Nome || '%'");

            sb.Append(" ORDER BY ROWNUM) MATERIAS");
            sb.Append(" WHERE ROWNUM <= :Itens AND MATERIAS.RN > (:Page -1) * :Itens");

            return _connection.Query<Materia>(sb.ToString(), new
            {
                //Nome = nome.ToUpperIgnoreNull(),
                Nome = nome,
                Itens = itens,
                Page = page
            });
            
        }

    }

}