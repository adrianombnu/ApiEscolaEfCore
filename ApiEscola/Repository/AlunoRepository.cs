using Dominio.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

#nullable enable
namespace ApiEscolaEfCore.Services
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

    }
}