using ApiEscola.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace ApiEscola.Repository
{
    public class TurmaRepository
    {
        private readonly IConfiguration _configuration;
        private List<Turma> _turmas;
        private List<Aluno> _alunos;
        private List<Materia> _materiasCurso;
        private List<Materia> _materiasAluno;

        public TurmaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }


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


    }
}
