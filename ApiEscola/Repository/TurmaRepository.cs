using ApiEscola.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ApiEscola.Repository
{
    public class TurmaRepository
    {
        private readonly IConfiguration _configuration;
        private List<Guid> _materiasCurso;
        private List<Turma> _turmas;

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
                                            (ID, NOME, dataInicio, dataFim, idCurso)
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
                                                            (IDTURMA, IDMATERIA)
                                                          VALUES(:IdTurma,:IdMateria)";

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

        public bool BuscaTurmaPeloNome(string nomeTurma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var cursoEncontrado = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"select * from turma where nome = :nome", conn);

                cmd.Parameters.Add(new OracleParameter("nome", nomeTurma));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    cursoEncontrado = true;

            }

            return cursoEncontrado;

        }

        public bool VerificaVinculoMateriaComATurma(Guid idMateria, Guid idTurma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var materiaVinculada = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"select * from turma_materia 
                                                            where idTurma = :IdTurma
                                                              and idMateria = :IdMateria ", conn);

                cmd.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                cmd.Parameters.Add(new OracleParameter("IdMateria", idMateria.ToString()));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    materiaVinculada = true;

            }

            return materiaVinculada;

        }       

        public Turma BuscaTurmaPeloId(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"select * from turma where id = :id", conn);

                cmd.Parameters.Add(new OracleParameter("id", id.ToString()));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //buscar materias da turma
                        using var cmdMateriasCurso = new OracleCommand(@"SELECT * FROM materia a WHERE a.ID IN (SELECT tm.IDMATERIA FROM turma_materia tm WHERE tm.IDTURMA = :idTurma)", conn);

                        cmdMateriasCurso.Parameters.Add(new OracleParameter("idTurma", reader["id"].ToString()));

                        using (var readerMateriasCurso = cmdMateriasCurso.ExecuteReader())
                        {
                            _materiasCurso = new List<Guid>();

                            while (readerMateriasCurso.Read())
                            {
                                _materiasCurso.Add(Guid.Parse(Convert.ToString(readerMateriasCurso["id"])));

                            }
                        }

                        return new Turma(Convert.ToString(reader["nome"]), Convert.ToDateTime(reader["dataInicio"]), Convert.ToDateTime(reader["dataFim"]), _materiasCurso, Guid.Parse(Convert.ToString(reader["id"])), Guid.Parse(Convert.ToString(reader["idCurso"])));

                    }
                }

            }

            return null;

        }

        public IEnumerable<Turma> ListarTurmas(string? nome = null, DateTime? dataInicio = null, DateTime? dataFim = null, int page = 1, int itens = 50)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmdTurma = new OracleCommand();

                var query = (@"select * from turma WHERE 1 = 1");

                var sb = new StringBuilder(query);

                if (!string.IsNullOrEmpty(nome))
                {
                    sb.Append(" AND nome like '%' || :Nome || '%'");
                    cmdTurma.Parameters.Add(new OracleParameter("Nome", nome));
                }

                if (!string.IsNullOrEmpty(dataInicio.ToString()))
                {
                    sb.Append(" AND to_char(dataInicio,'dd/mm/rrrr') = :DataInicio");
                    cmdTurma.Parameters.Add(new OracleParameter("DataInicio", dataInicio.Value.ToString("dd/MM/yyyy")));
                }

                if (!string.IsNullOrEmpty(dataFim.ToString()))
                {
                    sb.Append(" AND to_char(dataFim,'dd/mm/rrrr') = :DataFim");
                    cmdTurma.Parameters.Add(new OracleParameter("DataFim", dataFim.Value.ToString("dd/MM/yyyy")));
                }

                cmdTurma.Connection = conn;
                cmdTurma.CommandText = sb.ToString();

                using (var readerTurma = cmdTurma.ExecuteReader())
                {
                    _turmas = new List<Turma>();

                    while (readerTurma.Read())
                    {
                        //buscar materias da turma
                        using var cmdMateriasCurso = new OracleCommand(@"SELECT * FROM materia a WHERE a.ID IN (SELECT tm.IDMATERIA FROM turma_materia tm WHERE tm.IDTURMA = :idTurma)", conn);

                        cmdMateriasCurso.Parameters.Add(new OracleParameter("idTurma", readerTurma["id"].ToString()));

                        using (var readerMateriasCurso = cmdMateriasCurso.ExecuteReader())
                        {
                            _materiasCurso = new List<Guid>();

                            while (readerMateriasCurso.Read())
                            {
                                _materiasCurso.Add(Guid.Parse(Convert.ToString(readerMateriasCurso["id"])));

                            }
                        }

                        var turma = new Turma(Convert.ToString(readerTurma["nome"]), Convert.ToDateTime(readerTurma["dataInicio"]), Convert.ToDateTime(readerTurma["dataFim"]), _materiasCurso, Guid.Parse(Convert.ToString(readerTurma["id"])), Guid.Parse(Convert.ToString(readerTurma["idCurso"])));

                        _turmas.Add(turma);

                    }

                }
            }

            return _turmas.Skip((page - 1) * itens).Take(itens); ;
        }

    }

}
