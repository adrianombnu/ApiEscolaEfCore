using ApiEscolaEfCore.Entities;
using System;
using System.Collections.Generic;

namespace ApiEscolaEfCore.DTOs
{
    public class RetornoTurmaDTO
    {
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public List<RetornoAlunoDTO> Alunos { get; set; }
        public List<RetornoMateriaDTO> Materias { get; set; }
        public Guid IdCurso { get; set; }
        
    }
}
