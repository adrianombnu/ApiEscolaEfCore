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
            var turma =  _connection.QuerySingleOrDefault<Turma>(@"SELECT T.ID,
                                                                          T.NOME, 
                                                                          T.DATAINICIO,
                                                                          T.DATAFIM,
                                                                          T.IDCURSO
                                                                     FROM TURMA T
                                                                    WHERE T.ID = :Id", new { id = id.ToString() });

            turma.IdMaterias = _connection.Query<Guid>(@"SELECT A.ID 
                                                           FROM MATERIA A 
                                                          WHERE A.ID IN (SELECT TM.IDMATERIA 
                                                                           FROM turma_materia TM 
                                                                          WHERE TM.IDTURMA = :IdTurma)", new { IdTurma = id.ToString() }).AsList<Guid>();

            return turma;

        }

        public bool BuscarTurmaPeloNome(string nomeTurma, Guid idTurma, bool consideraIdDifente = false)
        {
            var query = (@"SELECT T.ID,
                                  T.NOME, 
                                  T.DATAINICIO,
                                  T.DATAFIM,
                                  T.IDCURSO 
                             FROM TURMA T WHERE 1 = 1");

            var sb = new StringBuilder(query);

            sb.Append(" AND UPPER(T.NOME) = :Nome ");

            if (consideraIdDifente)
                sb.Append(" AND T.ID <> :IdTurma ");

            var turma = _connection.QueryFirstOrDefault<Turma>(sb.ToString(), new { Nome = nomeTurma,
                                                                                            IdTurma = idTurma.ToString() });

            if (turma is null)
                return false;

            return true;

        }

        public bool VerificaVinculoMateriaComATurma(Guid idMateria, Guid idTurma)
        {
            var turma = _connection.QueryFirstOrDefault(@"SELECT * 
                                                            FROM TURMA_MATERIA TM
                                                        WHERE TM.IDTURMA = :IdTurma 
                                                            AND TM.IDMATERIA = :IdMateria", new
            {
                IdTurma = idTurma.ToString(),
                IdMateria = idMateria.ToString()
            });

            if (turma is null)
                return false;

            return true;

        }

        public bool VerificaSePossuiAlunoVinculado(Guid idTurma)
        {
            var quantidade = _connection.QuerySingleOrDefault(@"SELECT COUNT(ID) QUANTIDADE
                                                                  FROM turma_materia tm
                                                                 INNER JOIN ALUNO_MATERIA am
                                                                    ON tm.ID = am.IDTURMAMATERIA
                                                                 WHERE tm.IDTURMA = :IdTurma", new
            {
                IdTurma = idTurma.ToString()
            });

            if (quantidade > 0)
                return false;

            return true;

        }

        public bool VerificaAlunoMatriculadoMateria(Guid idMateria, Guid idTurma)
        {
            var quantidade = _connection.QuerySingleOrDefault(@"SELECT COUNT(ID) QUANTIDADE
                                                                  FROM turma_materia tm
                                                            INNER JOIN ALUNO_MATERIA am
                                                                    ON tm.ID = am.IDTURMAMATERIA
                                                                 WHERE tm.IDTURMA = :IdTurma
                                                                   AND IDMATERIA = :IdMateria", new
            {
                IdTurma = idTurma.ToString(),
                IdMateria = idMateria.ToString()
            });

            if (quantidade > 0)
                return false;

            return true;

        }

        public int BuscarQuantidadeMateriasCadastradas(Guid idTurma)
        {
            var quantidade = _connection.QuerySingleOrDefault(@"SELECT COUNT(ID) QUANTIDADE 
                                                                  FROM TURMA_MATERIA 
                                                                 WHERE IDTURMA = :IdTurma", new
            {
                IdTurma = idTurma.ToString() 
            });

            return quantidade;
             
        }

        public IEnumerable<Turma> ListarTurmas(string? nome = null, DateTime? dataInicio = null, DateTime? dataFim = null, int page = 1, int itens = 50)
        {
            var query = (@"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ROWID) AS RN,
                                                 T.ID,
                                                 T.NOME, 
                                                 T.DATAINICIO,
                                                 T.DATAFIM,
                                                 T.IDCURSO
                                            FROM TURMA T 
                                           WHERE 1 = 1");

            var sb = new StringBuilder(query);

            if (!string.IsNullOrEmpty(nome))
                sb.Append(" AND UPPER(T.NOME) LIKE '%' || :Nome || '%'");

            if (!string.IsNullOrEmpty(dataInicio.ToString()))
                sb.Append(" AND to_char(T.DATAINICIO,'dd/mm/rrrr') = :DataInicio");

            if (!string.IsNullOrEmpty(dataFim.ToString()))
                sb.Append(" AND to_char(T.DATAFIM,'dd/mm/rrrr') = :DataFim");

            sb.Append(" ) TURMAS");
            sb.Append(" WHERE ROWNUM <= :Itens AND TURMAS.RN > (:Page -1) * :Itens");

            var turmas = _connection.Query<Turma>(sb.ToString(), new
            {
                Nome = nome.ToUpperIgnoreNull(),
                DataInicio = (dataInicio.HasValue ? dataInicio.Value.ToString("dd/MM/yyyy") : "null"),
                DataFim = (dataFim.HasValue ? dataFim.Value.ToString("dd/MM/yyyy") : "null"),
                Itens = itens,
                Page = page
            });

            foreach (var turma in turmas)
            {
                turma.IdMaterias = _connection.Query<Guid>(@"SELECT A.ID 
                                                           FROM MATERIA A 
                                                          WHERE A.ID IN (SELECT TM.IDMATERIA 
                                                                           FROM TURMA_MATERIA TM 
                                                                          WHERE TM.IDTURMA = :IdTurma)", new { IdTurma = turma.Id.ToString() }).AsList<Guid>();

            }

            return turmas;
            
        }

        public IEnumerable<Aluno> BuscarAlunos(Guid idTurma)
        {
            var alunos = _connection.Query<Aluno>(@"SELECT DISTINCT AM.IDALUNO,
                                                                    AL.ID,
                                                                    AL.NOME, 
                                                                    AL.SOBRENOME,
                                                                    AL.DATADENASCIMENTO,
                                                                    AL.DOCUMENTO      
                                                      FROM turma a
                                                inner JOIN TURMA_MATERIA tm
                                                        ON a.ID = tm.IDTURMA
                                                inner JOIN materia M
                                                        ON M.ID = tm.IDMATERIA
                                                inner JOIN ALUNO_MATERIA am
                                                        ON am.IDTURMAMATERIA = tm.ID
                                                inner JOIN aluno AL
                                                        ON AL.ID = am.IDALUNO
                                                     WHERE a.ID = :IdTurma", new { IdTurma = idTurma.ToString() });

            foreach (var aluno in alunos)
            {
                var materias = _connection.Query<Guid>(@"SELECT tm.IDMATERIA
                                                           FROM turma a
                                                     inner JOIN TURMA_MATERIA tm 
                                                             ON a.ID = tm.IDTURMA
                                                     inner JOIN ALUNO_MATERIA am 
                                                             ON am.IDTURMAMATERIA = tm.ID
                                                     inner JOIN aluno AL 
	                                                         ON AL.ID = am.IDALUNO	 
	                                                      WHERE tm.IDTURMA = :IdTurma
                                                            AND AL.ID = :IdAluno", new { IdTurma = idTurma.ToString(),
                                                                                          IdAluno = aluno.Id}).AsList<Guid>();
                aluno.IdMaterias = materias;
            }

                 
            return alunos;

        }

    }

}