using System;
using System.Collections.Generic;

namespace ApiEscola.Entities
{
    public class Turma : Base
    {
        public Turma(string nome, DateTime dataInicio, DateTime dataFim, List<Aluno> alunos, List<Materia> materias, Guid idCurso)
        {
            Nome = nome;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Alunos = alunos;
            Materias = materias;
            Id = Guid.NewGuid();
            IdCurso = idCurso;
        
        }

        public Turma(string nome, DateTime dataInicio, DateTime dataFim, List<Aluno> alunos, List<Materia> materias, Guid idTurma, Guid idCurso)
        {
            Nome = nome;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Alunos = alunos;
            Materias = materias;
            Id = idTurma;
            IdCurso = idCurso;
        }

        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public List<Aluno> Alunos { get; set; }
        public List<Materia> Materias { get; set; }
        public Guid IdCurso { get; set; }

    }
}
