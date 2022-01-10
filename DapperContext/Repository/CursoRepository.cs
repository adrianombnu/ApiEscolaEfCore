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
            return _connection.QuerySingleOrDefault<Curso>(
                @"SELECT ID,
                         NOME, 
                         DESCRICAO
                    FROM CURSO 
                   WHERE ID = :Id", new { id });
        }

        public bool VerificaSeCursoPossuiTurma(Guid idCurso)
        {
            var possuiTurma = _connection.QueryFirstOrDefault<Curso>(@"SELECT C.ID,
                                                                              C.NOME,
                                                                              C.DESCRICAO
                                                                         FROM CURSO C 
                                                                     INNER JOIN TURMA T 
                                                                             ON C.ID = T.IDCURSO 
                                                                          WHERE C.ID  = :IdCurso", new { IdCurso = idCurso.ToString() });

            if (possuiTurma is null)
                return false;

            return true;

        }

        public IEnumerable<Curso> ListarCursos(string? nome = null, string? descricao = null, int page = 1, int itens = 50)
        {
            var query = (@"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ROWID) AS RN,
                                                 C.ID,
                                                 C.NOME,
                                                 C.DESCRICAO
                                            FROM CURSO C 
                                           WHERE 1 = 1");

            var sb = new StringBuilder(query);

            if (!string.IsNullOrEmpty(nome))
                sb.Append(" AND UPPER(C.NOME) LIKE '%' || :Nome || '%'");

            if (!string.IsNullOrEmpty(descricao))
                sb.Append(" AND UPPER(C.DESCRICAO) LIKE '%' || :Descricao || '%'");

            sb.Append(" ) CURSOS");
            sb.Append(" WHERE ROWNUM <= :Itens AND CURSOS.RN > (:Page -1) * :Itens");

            return _connection.Query<Curso>(sb.ToString(), new
            {
                Nome = nome.ToUpperIgnoreNull(),
                Descricao = descricao.ToUpperIgnoreNull(),
                Itens = itens,
                Page = page
            });
            
        }

        public bool VerificaSeCursoJaCadastrado(string nomeCurso, Guid id)
        { 
            var curso = _connection.QuerySingleOrDefault<Curso>(@"SELECT C.ID,
                                                                         C.NOME,
                                                                         C.DESCRICAO
                                                                    FROM CURSO C
                                                                     WHERE UPPER(C.NOME) = :Nome 
                                                                       AND C.ID <> :Id", new { Nome = nomeCurso,
                                                                                               Id = id.ToString()});

            if (curso is null)
                return false;

            return true;

        }

        public bool BuscaCursoPeloNome(string nomeCurso)
        {
            var curso = _connection.QueryFirstOrDefault<Curso>(@"SELECT C.ID,
                                                                        C.NOME,
                                                                        C.DESCRICAO
                                                                   FROM CURSO C 
                                                                     WHERE UPPER(C.NOME) = :Nome", new{
                                                                                                        Nome = nomeCurso
                                                                                                    });

            if (curso is null)
                return false;

            return true;

        }

    }

}