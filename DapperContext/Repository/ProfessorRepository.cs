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
            return _connection.QuerySingleOrDefault<Professor>(
                @"SELECT * FROM PROFESSOR WHERE ID = :Id", new { id });
        }

        public bool BuscaProfessorPeloDocumento(string documento)
        {
            var professor = _connection.QueryFirstOrDefault<Professor>(@"SELECT * FROM PROFESSOR 
                                                            WHERE DOCUMENTO = :Documento", new { Documento = documento });

            if (professor is null)
                return false;

            return true;

        }

        public bool VerificaSePossuiMateriaVinculada(Guid id)
        {
            var professor = _connection.QueryFirstOrDefault<Professor>(@"SELECT * FROM PROFESSOR P
                                                                     INNER JOIN MATERIA M 
                                                                             ON P.id = M.idProfessor 
                                                                          WHERE P.ID = :IdProfessor", new { IdProfessor = id.ToString() });

            if (professor is null)
                return false;

            return true;

        }

        public bool VerificaSeProfessorJaCadastrado(string documento, Guid id)
        {
            var professor = _connection.QuerySingleOrDefault<Professor>(@"SELECT * FROM PROFESSOR 
                                                                          WHERE ID <> :Id 
                                                                            AND DOCUMENTO = :Documento", new { Documento = documento });

            if (professor is null)
                return false;

            return true;

        }

        public IEnumerable<Professor> ListarProfessores(string? nome = null, string? sobrenome = null, DateTime? dataDeNascimento = null, string? documento = null, int page = 1, int itens = 50)
        {
            var query = (@"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ROWID) AS RN,
                                                 P.* 
                                            FROM PROFESSOR P 
                                           WHERE 1 = 1");

            var sb = new StringBuilder(query);

            if (!string.IsNullOrEmpty(nome))
                sb.Append(" AND UPPER(P.NOME) LIKE '%' || :Nome || '%' ");

            if (!string.IsNullOrEmpty(sobrenome))
                sb.Append(" AND UPPER(P.SOBRENOME) LIKE '%' || :Sobrenome || '%' ");

            if (!string.IsNullOrEmpty(dataDeNascimento.ToString()))
                sb.Append(" AND to_char(P.DATADENASCIMENTO,'dd/mm/rrrr') = :DataDeNascimento ");

            if (!string.IsNullOrEmpty(documento))
                sb.Append(" AND P.DOCUMENTO = :Documento");

            sb.Append(" ) ALUNOS");
            sb.Append(" WHERE ROWNUM <= :Itens AND ALUNOS.RN > (:Page -1) * :Itens");

            return _connection.Query<Professor>(sb.ToString(), new
            {
                //Nome = nome.ToUpperIgnoreNull(),
                Nome = nome,
                //Sobrenome = sobrenome.ToUpperIgnoreNull(),
                Sobrenome = sobrenome,
                DataDeNascimento = (dataDeNascimento.HasValue ? dataDeNascimento.Value.ToString("dd/MM/yyyy") : "null"),
                Documento = documento,
                Itens = itens,
                Page = page
            });                        

        }

    }

}