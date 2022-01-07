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
    public class AlunoRepository : RepositoryBase<Guid, Aluno>, IAlunoRepository
    {
        private readonly List<Aluno> _alunos;

        public AlunoRepository(IConfiguration configuration) : base(configuration)
        {
            _alunos ??= new List<Aluno>();
        }

        public override Aluno BuscarPeloId(Guid IdAluno)
        {
            var aluno = _connection.QuerySingleOrDefault<Aluno>(@"SELECT * 
                                                                    FROM ALUNO 
                                                                   WHERE ID = :IdAluno", new { IdAluno = IdAluno.ToString() });

            aluno.IdMaterias = _connection.Query<Guid>(@"SELECT IDTURMAMATERIA 
                                                           FROM ALUNO_MATERIA 
                                                          WHERE IDALUNO = :IdAluno", new { IdAluno = IdAluno.ToString()}).AsList<Guid>();

            return aluno;

        }

        public bool VerificaSeAlunoJaCadastrado(string documento, Guid idTurma)
        {
            var query = (@"SELECT A.ID 
                             FROM ALUNO A 
                       INNER JOIN TURMA_ALUNO TA
                               ON A.ID = TA.IDALUNO 
                            WHERE A.DOCUMENTO = :Documento 
                              AND TA.IDTURMA = :IdTurma");

            var aluno = _connection.QueryFirstOrDefault<Materia>(query, new
            {
                Documento = documento.ToString(),
                idTurma = idTurma.ToString()
            });

            if (aluno is null)
                return false;

            return true;

        }

        public IEnumerable<Aluno> ListarAlunos(string? nome = null, string? sobrenome = null, DateTime? dataDeNascimento = null, int page = 1, int itens = 50)
        {
            var query = (@"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ROWID) AS RN,
                                                 A.*                                                
                                            FROM ALUNO A 
                                           WHERE 1 = 1");

            var sb = new StringBuilder(query);

            if (!string.IsNullOrEmpty(nome))
                sb.Append(" AND UPPER(A.NOME) LIKE '%' || :Nome || '%' ");

            if (!string.IsNullOrEmpty(sobrenome))
                sb.Append(" AND UPPER(A.SOBRENOME) LIKE '%' || :Sobrenome || '%' ");

            if (!string.IsNullOrEmpty(dataDeNascimento.ToString()))
                sb.Append(" AND to_char(A.DATADENASCIMENTO,'dd/mm/rrrr') = :DataDeNascimento ");

            sb.Append(" ) ALUNOS");
            sb.Append(" WHERE ROWNUM <= :Itens AND ALUNOS.RN > (:Page -1) * :Itens");

            var alunos = _connection.Query<Aluno>(sb.ToString(), new
            {
                //Nome = nome.ToUpperIgnoreNull(),
                Nome = nome,
                //Sobrenome = sobrenome.ToUpperIgnoreNull(),
                Sobrenome = sobrenome,
                DataDeNascimento = (dataDeNascimento.HasValue ? dataDeNascimento.Value.ToString("dd/MM/yyyy") : "null"),
                Itens = itens,
                Page = page
            });

            query = (@"SELECT TM.IDMATERIA
                             FROM ALUNO_MATERIA AM
                       INNER JOIN TURMA_MATERIA TM
                               ON AM.IDTURMAMATERIA = TM.ID
                            WHERE AM.IDALUNO = :IdAluno");
                        
            foreach (var aluno in alunos)
            {
                var materias = _connection.Query<Guid>(query, new
                {
                    IdAluno = aluno.Id.ToString()
                }).AsList<Guid>();

                foreach (var materia in materias)
                {
                    aluno.IdMaterias ??= new List<Guid>();
                    aluno.IdMaterias.Add(materia);

                }

            }
            
            return alunos;

        }

    }

}