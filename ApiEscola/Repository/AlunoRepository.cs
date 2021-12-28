using ApiEscola.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

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

        public bool Cadastrar(Aluno aluno)
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
                                            (id, nome, sobrenome, dataDeNascimento, documento)
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

                    commandAddTurmaAluno.Parameters.Add(new OracleParameter("IdTurma", aluno.IdTurma.ToString()));
                    commandAddTurmaAluno.Parameters.Add(new OracleParameter("IdAluno", aluno.Id.ToString()));

                    rows = commandAddTurmaAluno.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível vincular o aluno a turma.");

                    foreach (var id in aluno.IdMaterias)
                    {
                        using OracleCommand commandAddMateria = conn.CreateCommand();
                        commandAddMateria.Transaction = transaction;

                        commandAddMateria.CommandText = @"INSERT INTO APPACADEMY.aluno_materia
                                                            (IDALUNO, IDMATERIA)
                                                          VALUES(:IdAluno,:IdMateria)";

                        commandAddMateria.Parameters.Add(new OracleParameter("IdAluno", aluno.Id.ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdMateria", id.ToString()));

                        rows = commandAddMateria.ExecuteNonQuery();

                        if (rows == 0)
                            throw new Exception("Não foi possível vincular aluno a matéria.");

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

                using var cmd = new OracleCommand(@"SELECT * FROM aluno a INNER JOIN TURMA_ALUNO ta ON a.ID = ta.IDALUNO WHERE a.DOCUMENTO = :Documento AND ta.IDTURMA = :IdTurma;", conn);

                cmd.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                cmd.Parameters.Add(new OracleParameter("Documento", documento));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    cursoEncontrado = true;

            }

            return cursoEncontrado;

        }

        public Aluno BuscarAlunoPeloId(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            List<Guid> materias = new List<Guid>();

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"select * from aluno where id = :id", conn);

                cmd.Parameters.Add(new OracleParameter("id", id.ToString()));

                using (var reader = cmd.ExecuteReader())
                {                 
                    while (reader.Read())
                    {
                        using var cmdMaterias = new OracleCommand(@"select * from aluno_materia where idAluno = :id", conn);

                        cmdMaterias.Parameters.Add(new OracleParameter("id", id.ToString()));

                        using (var readerMaterias = cmdMaterias.ExecuteReader())
                        {
                            while (readerMaterias.Read())
                            {
                                materias.Add(Guid.Parse(Convert.ToString(reader["idMateria"])));

                            }
                        }

                        return new Aluno(Convert.ToString(reader["nome"]), Convert.ToString(reader["sobrenome"]), Convert.ToDateTime(reader["dataDeNascimento"]), Convert.ToString(reader["documento"]), materias, Guid.Parse(Convert.ToString(reader["idTurma"])));

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
                                          WHERE idTurma = :IdTurma
                                            AND idAluno = :IdAluno)";

                    commandTurmaAluno.Parameters.Add(new OracleParameter("IdTurma", aluno.IdTurma.ToString()));
                    commandTurmaAluno.Parameters.Add(new OracleParameter("IdAluno", aluno.Id.ToString()));

                    var rows = commandTurmaAluno.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível remover o vinculo do aluno com a turma.");

                    foreach (var id in aluno.IdMaterias)
                    {
                        using OracleCommand commandMateria = conn.CreateCommand();
                        commandMateria.Transaction = transaction;

                        commandMateria.CommandText = @"DELETE FROM APPACADEMY.aluno_materia
                                                             WHERE idAluno = :IdAluno
                                                               AND idMateria = :IdMateria";

                        commandMateria.Parameters.Add(new OracleParameter("IdAluno", aluno.Id.ToString()));
                        commandMateria.Parameters.Add(new OracleParameter("IdMateria", id.ToString()));

                        rows = commandMateria.ExecuteNonQuery();

                        if (rows == 0)
                            throw new Exception("Não foi possível remover o vinculo do aluno com a matéria.");

                    }

                    using OracleCommand commandAluno = conn.CreateCommand();
                    commandAluno.Transaction = transaction;

                    commandAluno.CommandText = @"DELETE FROM APPACADEMY.aluno
                                                       WHERE ID = :IdAluno)";

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