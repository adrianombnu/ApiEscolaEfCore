using ApiEscola.Entities;
using System;
using System.Collections.Generic;

namespace ApiEscola.DTOs
{
    public class TurmaAlunoDTO
    {
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public List<Aluno> Alunos { get; set; }
        public Guid IdCurso { get; set; }
        
    }
}
