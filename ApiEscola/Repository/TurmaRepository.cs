using Dominio.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

#nullable enable
namespace ApiEscolaEfCore.Repository
{
    public class TurmaRepository
    {
        private readonly IConfiguration _configuration;
        private List<Guid> _materiasCurso;
        private List<Turma> _turmas;
        private List<Aluno> _alunos;

        public TurmaRepository(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public bool Cadastrar(Turma turma)
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
                    command.CommandText = @"INSERT INTO APPACADEMY.turma
                                            (ID, NOME, DATAINICIO, DATAFIM, IDCURSO)
                                          VALUES(:Id,:Nome,:DataInicio,:DataFim, :IdCurso)";

                    command.Parameters.Add(new OracleParameter("Id", turma.Id.ToString()));
                    command.Parameters.Add(new OracleParameter("Nome", turma.Nome));
                    command.Parameters.Add(new OracleParameter("DataInicio", turma.DataInicio));
                    command.Parameters.Add(new OracleParameter("DataFim", turma.DataFim));
                    command.Parameters.Add(new OracleParameter("IdCurso", turma.IdCurso.ToString()));

                    var rows = command.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível cadastrar a turma.");

                    foreach (var id in turma.IdMaterias)
                    {
                        using OracleCommand commandAddMateria = conn.CreateCommand();
                        commandAddMateria.Transaction = transaction;

                        commandAddMateria.CommandText = @"INSERT INTO APPACADEMY.turma_materia
                                                            (ID, IDTURMA, IDMATERIA)
                                                          VALUES(:Id, :IdTurma, :IdMateria)";

                        commandAddMateria.Parameters.Add(new OracleParameter("Id", Guid.NewGuid().ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdTurma", turma.Id.ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdMateria", id.ToString()));

                        rows = commandAddMateria.ExecuteNonQuery();

                        if (rows == 0)
                            throw new Exception("Não foi possível cadastrar a turma.");

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

        public bool AtualizarTurma(Turma turma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(
                    @"UPDATE APPACADEMY.turma
                        SET ID = :Id,
                            NOME = :Nome, 
                            DATAINICIO = :DataInicio,
                            DATAFIM = :DataFim,
                            IDCURSO = :IdCurso
                      WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", turma.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", turma.Nome));
                cmd.Parameters.Add(new OracleParameter("DataInicio", turma.DataInicio));
                cmd.Parameters.Add(new OracleParameter("DataFim", turma.DataFim));
                cmd.Parameters.Add(new OracleParameter("IdCurso", turma.IdCurso.ToString()));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool RomoverTurma(Guid id)
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
                    command.CommandText = @"DELETE FROM APPACADEMY.turma_materia
                                                  WHERE IDTURMA = :IdTurma ";

                    command.Parameters.Add(new OracleParameter("Id", id.ToString()));

                    var rows = command.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível remover as matérias vinculadas a turma.");

                    using OracleCommand commandAddMateria = conn.CreateCommand();
                    commandAddMateria.Transaction = transaction;

                    commandAddMateria.CommandText = @"DELETE FROM APPACADEMY.turma
                                                            WHERE ID = :IdTurma ";

                    commandAddMateria.Parameters.Add(new OracleParameter("IdTurma", id.ToString()));

                    rows = commandAddMateria.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível remover a turma.");


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

        public bool AdicionarMaterias(Guid idTurma, List<Guid> idMaterias)
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
                    foreach (var id in idMaterias)
                    {
                        using OracleCommand commandAddMateria = conn.CreateCommand();
                        commandAddMateria.Transaction = transaction;

                        commandAddMateria.CommandText = @"INSERT INTO APPACADEMY.turma_materia
                                                            (ID, IDTURMA, IDMATERIA)
                                                          VALUES(:Id, :IdTurma, :IdMateria)";

                        commandAddMateria.Parameters.Add(new OracleParameter("Id", Guid.NewGuid().ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdMateria", id.ToString()));

                        var rows = commandAddMateria.ExecuteNonQuery();

                        if (rows == 0)
                            throw new Exception("Não foi possível adicionar a matéria.");

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

        public bool RemoverMaterias(Guid idTurma, List<Guid> idMaterias)
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
                    foreach (var id in idMaterias)
                    {
                        using OracleCommand commandAddMateria = conn.CreateCommand();
                        commandAddMateria.Transaction = transaction;

                        commandAddMateria.CommandText = @"DELETE FROM APPACADEMY.turma_materia
                                                                WHERE IDTURMA = :IdTurma
                                                                  AND IDMATERIA = :IdMateria";

                        commandAddMateria.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdMateria", id.ToString()));

                        var rows = commandAddMateria.ExecuteNonQuery();

                        if (rows == 0)
                            throw new Exception("Não foi possível remover a matéria.");

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
                       

    }   

}
