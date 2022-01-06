using System;
using System.Collections.Generic;

namespace Dominio.Entities
{
    public class Turma : Base<Guid>
    {
        public Turma(string nome, DateTime dataInicio, DateTime dataFim, List<Guid> idMaterias, Guid idCurso)
        {
            Nome = nome;
            DataInicio = dataInicio;
            DataFim = dataFim;
            IdMaterias = idMaterias;
            Id = Guid.NewGuid();
            IdCurso = idCurso;
        
        }

        public Turma(string nome, DateTime dataInicio, DateTime dataFim, List<Guid> idMaterias, Guid idTurma, Guid idCurso)
        {
            Nome = nome;
            DataInicio = dataInicio;
            DataFim = dataFim;
            IdMaterias = idMaterias;
            Id = idTurma;
            IdCurso = idCurso;
        }

        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public List<Guid> IdMaterias { get; set; }
        public Guid IdCurso { get; set; }

    }
}
