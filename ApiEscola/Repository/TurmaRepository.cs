using ApiEscola.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace ApiEscola.Repository
{
    public class TurmaRepository
    {
        private readonly IConfiguration _configuration;
        private List<Aluno> _alunos;
        private List<Materia> _materiasCurso;
        private List<Materia> _materiasAluno;
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
                        return new Turma(Convert.ToString(reader["nome"]), Convert.ToDateTime(reader["dataInicio"]), Convert.ToDateTime(reader["dataFim"]), null, Guid.Parse(Convert.ToString(reader["idTurma"])), Guid.Parse(Convert.ToString(reader["idCurso"])));
                        
                    }
                }

            }

            return null;

        }

        /*
        public IEnumerable<Turma> ListarTurmas()
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmdTurma = new OracleCommand(@"select * from turma", conn);

                using (var readerTurma = cmdTurma.ExecuteReader())
                {
                    _turmas = new List<Turma>();

                    while (readerTurma.Read())
                    {
                        //buscar alunos da turma
                        using var cmdAlunos = new OracleCommand(@"SELECT * FROM aluno a WHERE a.ID IN (SELECT ta.IDALUNO FROM turma_aluno ta WHERE ta.IDTURMA = :idTurma)", conn);

                        cmdAlunos.Parameters.Add(new OracleParameter("idTurma", readerTurma["id"].ToString()));

                        using (var readerAlunos = cmdAlunos.ExecuteReader())
                        {
                            _alunos = new List<Aluno>();

                            while (readerAlunos.Read())
                            {
                                //buscar materias do aluno
                                using var cmdMateriasAluno = new OracleCommand(@"SELECT * FROM materia a WHERE a.ID IN (SELECT tm.IDMATERIA FROM turma_materia tm WHERE tm.IDTURMA = :idTurma)", conn);

                                cmdMateriasAluno.Parameters.Add(new OracleParameter("idAluno", readerAlunos["id"].ToString()));

                                using (var readerMateriasAluno = cmdMateriasAluno.ExecuteReader())
                                {
                                    _materiasAluno = new List<Materia>();

                                    while (readerMateriasAluno.Read())
                                    {
                                        var materia = new Materia(Convert.ToString(readerMateriasAluno["nome"]), Guid.Parse(Convert.ToString(readerMateriasAluno["idProfessor"])), Guid.Parse(Convert.ToString(readerMateriasAluno["id"])));
                                        _materiasAluno.Add(materia);

                                    }
                                }

                                var aluno = new Aluno(Convert.ToString(readerAlunos["nome"]), Convert.ToString(readerAlunos["sobrenome"]), Convert.ToDateTime(readerAlunos["dataDeNascimento"]), Convert.ToString(readerAlunos["documento"]), _materiasAluno, Guid.Parse(Convert.ToString(readerAlunos["id"])));

                                _alunos.Add(aluno);

                            }
                        }

                        //buscar materias da turma
                        using var cmdMateriasCurso = new OracleCommand(@"SELECT * FROM materia a WHERE a.ID IN (SELECT tm.IDMATERIA FROM turma_materia tm WHERE tm.IDTURMA = :idTurma)", conn);

                        cmdMateriasCurso.Parameters.Add(new OracleParameter("idTurma", readerTurma["id"].ToString()));

                        using (var readerMateriasCurso = cmdMateriasCurso.ExecuteReader())
                        {
                            _materiasCurso = new List<Materia>();

                            while (readerMateriasCurso.Read())
                            {
                                var materia = new Materia(Convert.ToString(readerMateriasCurso["nome"]), Guid.Parse(Convert.ToString(readerMateriasCurso["idProfessor"])), Guid.Parse(Convert.ToString(readerMateriasCurso["id"])));
                                _materiasCurso.Add(materia);

                            }
                        }

                        var turma = new Turma(Convert.ToString(readerTurma["nome"]), Convert.ToDateTime(readerTurma["dataInicio"]), Convert.ToDateTime(readerTurma["dataFim"]), _alunos, _materiasCurso, Guid.Parse(Convert.ToString(readerTurma["id"])), Guid.Parse(Convert.ToString(readerTurma["idCurso"])));

                        _turmas.Add(turma);

                    }
                }
            }

            return _turmas;

        }
        */

    }
}
