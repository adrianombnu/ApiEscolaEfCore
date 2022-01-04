using ApiEscola.Entities;
using ApiEscola.Extensions;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ApiEscola.Services
{
    public class AlunoRepository
    {
        private readonly IConfiguration _configuration;
        private readonly List<Aluno> _alunos;

        public AlunoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _alunos ??= new List<Aluno>();

        }

        public bool Cadastrar(Guid idTurma, Aluno aluno)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using OracleCommand command = conn.CreateCommand();
                OracleTransaction transaction;

                // Start a local transaction
                transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                // Assign transaction object for a pending local transaction
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"INSERT INTO APPACADEMY.aluno
                                            (ID, NOME, SOBRENOME, DATADENASCIMENTO, DOCUMENTO)
                                          VALUES(:Id,:Nome,:Sobrenome, :DataDeNascimento,:Documento)";

                    command.Parameters.Add(new OracleParameter("Id", aluno.Id.ToString()));
                    command.Parameters.Add(new OracleParameter("Nome", aluno.Nome));
                    command.Parameters.Add(new OracleParameter("Sobrenome", aluno.Sobrenome));
                    command.Parameters.Add(new OracleParameter("DataDeNascimento", aluno.DataNascimento));
                    command.Parameters.Add(new OracleParameter("Documento", aluno.Documento));

                    var rows = command.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível cadastrar o aluno.");

                    using OracleCommand commandAddTurmaAluno = conn.CreateCommand();
                    commandAddTurmaAluno.Transaction = transaction;

                    commandAddTurmaAluno.CommandText = @"INSERT INTO APPACADEMY.turma_aluno
                                                            (IDTURMA, IDALUNO)
                                                          VALUES(:IdTurma,:IdAluno)";

                    commandAddTurmaAluno.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                    commandAddTurmaAluno.Parameters.Add(new OracleParameter("IdAluno", aluno.Id.ToString()));

                    rows = commandAddTurmaAluno.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível vincular o aluno a turma.");

                    foreach (var idMateria in aluno.IdMaterias)
                    {
                        using OracleCommand commandConsultaTurmaMateria = conn.CreateCommand();
                        commandConsultaTurmaMateria.Transaction = transaction;

                        commandConsultaTurmaMateria.CommandText = @"SELECT * FROM TURMA_MATERIA TM
                                                                            WHERE TM.IDTURMA = :IdTurma
                                                                              AND TM.IDMATERIA = :IdMateria";

                        commandConsultaTurmaMateria.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                        commandConsultaTurmaMateria.Parameters.Add(new OracleParameter("IdMateria", idMateria.ToString()));

                        using (var readerConsultaTurmaMateria = commandConsultaTurmaMateria.ExecuteReader())
                        {
                            if (!readerConsultaTurmaMateria.HasRows)
                                throw new Exception("Matéria não está vinculada a turma informada.");

                            while (readerConsultaTurmaMateria.Read())
                            {
                                var id = Convert.ToString(readerConsultaTurmaMateria["id"]);

                                using OracleCommand commandAddMateria = conn.CreateCommand();
                                commandAddMateria.Transaction = transaction;

                                commandAddMateria.CommandText = @"INSERT INTO APPACADEMY.aluno_materia
                                                                        (IDALUNO, IDTURMAMATERIA)
                                                                      VALUES(:IdAluno,:IdTurmaMateria)";

                                commandAddMateria.Parameters.Add(new OracleParameter("IdAluno", aluno.Id.ToString()));
                                commandAddMateria.Parameters.Add(new OracleParameter("IdTurmaMateria", id.ToString()));

                                rows = commandAddMateria.ExecuteNonQuery();

                                if (rows == 0)
                                    throw new Exception("Não foi possível vincular aluno a matéria.");

                            }
                        }

                    }

                    transaction.Commit();
                    retorno = true;

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    retorno = false;
                }

            }

            return retorno;

        }

        public bool VerificaSeAlunoJaCadastrado(string documento, Guid idTurma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var cursoEncontrado = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM ALUNO A 
                                                       INNER JOIN TURMA_ALUNO TA
                                                               ON A.ID = TA.IDALUNO 
                                                            WHERE A.DOCUMENTO = :Documento 
                                                              AND TA.IDTURMA = :IdTurma", conn);

                cmd.Parameters.Add(new OracleParameter("Documento", documento));
                cmd.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    cursoEncontrado = true;

            }

            return cursoEncontrado;

        }

        public Aluno BuscarAlunoPeloId(Guid idAluno)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            List<Guid> materias = new List<Guid>();

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM ALUNO WHERE ID = :IdAluno", conn);

                cmd.Parameters.Add(new OracleParameter("IdAluno", idAluno.ToString()));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        using var cmdMaterias = new OracleCommand(@"SELECT * FROM ALUNO_MATERIA WHERE IDALUNO = :IdAluno", conn);

                        cmdMaterias.Parameters.Add(new OracleParameter("IdAluno", idAluno.ToString()));

                        using (var readerMaterias = cmdMaterias.ExecuteReader())
                        {
                            while (readerMaterias.Read())
                            {
                                materias.Add(Guid.Parse(Convert.ToString(readerMaterias["idTurmaMateria"])));

                            }
                        }

                        return new Aluno(Convert.ToString(reader["nome"]), Convert.ToString(reader["sobrenome"]), Convert.ToDateTime(reader["dataDeNascimento"]), Convert.ToString(reader["documento"]), materias, Guid.Parse(Convert.ToString(reader["id"])));

                    }
                }

            }

            return null;

        }

        public bool RomoverAluno(Aluno aluno)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using OracleCommand commandTurmaAluno = conn.CreateCommand();
                OracleTransaction transaction;

                // Start a local transaction
                transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                // Assign transaction object for a pending local transaction
                commandTurmaAluno.Transaction = transaction;

                try
                {
                    commandTurmaAluno.CommandText = @"DELETE FROM APPACADEMY.turma_aluno                                            
                                                            WHERE IDALUNO = :IdAluno";

                    commandTurmaAluno.Parameters.Add(new OracleParameter("IdAluno", aluno.Id.ToString()));

                    var rows = commandTurmaAluno.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível remover o vinculo do aluno com a turma.");

                    foreach (var id in aluno.IdMaterias)
                    {
                        using OracleCommand commandMateria = conn.CreateCommand();
                        commandMateria.Transaction = transaction;

                        commandMateria.CommandText = @"DELETE FROM APPACADEMY.aluno_materia
                                                             WHERE IDALUNO = :IdAluno
                                                               AND IDTURMAMATERIA = :IdTurmaMateria";

                        commandMateria.Parameters.Add(new OracleParameter("IdAluno", aluno.Id.ToString()));
                        commandMateria.Parameters.Add(new OracleParameter("IdTurmaMateria", id.ToString()));

                        rows = commandMateria.ExecuteNonQuery();

                        if (rows == 0)
                            throw new Exception("Não foi possível remover o vinculo do aluno com a matéria.");

                    }

                    using OracleCommand commandAluno = conn.CreateCommand();
                    commandAluno.Transaction = transaction;

                    commandAluno.CommandText = @"DELETE FROM APPACADEMY.aluno
                                                       WHERE ID = :IdAluno";

                    commandAluno.Parameters.Add(new OracleParameter("IdAluno", aluno.Id.ToString()));

                    rows = commandAluno.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível remover o aluno.");

                    transaction.Commit();
                    retorno = true;

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    retorno = false;
                }

            }

            return retorno;

        }

        public IEnumerable<Aluno> ListarAlunos(string? nome = null, string? sobrenome = null, DateTime? dataDeNascimento = null, int page = 1, int itens = 50)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                var query = (@"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ROWID) AS RN,
                                                     A.*,                                                
                                                FROM ALUNO A WHERE 1 = 1");

                var sb = new StringBuilder(query);

                if (!string.IsNullOrEmpty(nome))
                    sb.Append(" AND UPPER(A.NOME) LIKE '%' || :Nome || '%' ");

                if (!string.IsNullOrEmpty(sobrenome))
                    sb.Append(" AND UPPER(A.SOBRENOME) LIKE '%' || :Sobrenome || '%' ");

                if (!string.IsNullOrEmpty(dataDeNascimento.ToString()))
                    sb.Append(" AND to_char(A.DATADENASCIMENTO,'dd/mm/rrrr') = :DataDeNascimento ");

                sb.Append(" ) ALUNOS");
                sb.Append(" WHERE ROWNUM <= :Itens AND ALUNOS.RN > (:Page -1) * :Itens");

                using var cmd = new OracleCommand(sb.ToString(), conn);

                //Esse bind serve para que quando, for passado mais parametros do que o necessário para montar o comando sql, devido a ser criado de forma dinamica, vamos evitar que dê
                //problema de quantidade maior ou a menor
                cmd.BindByName = true;

                cmd.Parameters.Add(new OracleParameter("Nome", nome.ToUpperIgnoreNull()));
                cmd.Parameters.Add(new OracleParameter("Sobrenome", sobrenome.ToUpperIgnoreNull()));
                cmd.Parameters.Add(new OracleParameter("DataDeNascimento", (dataDeNascimento.HasValue ? dataDeNascimento.Value.ToString("dd/MM/yyyy") : "null")));
                cmd.Parameters.Add(new OracleParameter("Itens", itens));
                cmd.Parameters.Add(new OracleParameter("Page", page));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<Guid> materias = new List<Guid>();

                        using var cmdMaterias = new OracleCommand(@"SELECT TM.IDMATERIA
                                                                      FROM ALUNO_MATERIA AM
                                                                INNER JOIN TURMA_MATERIA TM
                                                                        ON AM.IDTURMAMATERIA = TM.ID
                                                                     WHERE AM.IDALUNO = :IdAluno", conn);

                        cmdMaterias.Parameters.Add(new OracleParameter("IdAluno", Convert.ToString(reader["id"])));

                        using (var readerMaterias = cmdMaterias.ExecuteReader())
                        {
                            while (readerMaterias.Read())
                            {
                                materias.Add(Guid.Parse(Convert.ToString(readerMaterias["idMateria"])));

                            }
                        }

                        var aluno = new Aluno(Convert.ToString(reader["nome"]), Convert.ToString(reader["sobrenome"]), Convert.ToDateTime(reader["dataDeNascimento"]), Convert.ToString(reader["documento"]), materias, Guid.Parse(Convert.ToString(reader["id"])));

                        _alunos.Add(aluno);

                    }
                }
            }

            //return _alunos.Skip((page - 1) * itens).Take(itens);
            return _alunos;

        }

    }
}