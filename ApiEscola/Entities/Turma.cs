using System;
using System.Collections.Generic;

namespace ApiEscola.Entities
{
    public class Turma : Base
    {
        public Turma(string nome, DateTime dataInicio, DateTime dataFim, List<AlunoDTO> alunos, List<MateriaDTO> materias)
        {
            Nome = nome;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Alunos = alunos;
            Materias = materias;
            IdCurso = Guid.NewGuid();
        
        }

        public Turma(string nome, DateTime dataInicio, DateTime dataFim, List<AlunoDTO> alunos, List<MateriaDTO> materias, Guid idCurso)
        {
            Nome = nome;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Alunos = alunos;
            Materias = materias;
            IdCurso = idCurso;
        }

        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public List<AlunoDTO> Alunos { get; set; }
        public List<MateriaDTO> Materias { get; set; }
        public Guid IdCurso { get; set; }

    }
}
