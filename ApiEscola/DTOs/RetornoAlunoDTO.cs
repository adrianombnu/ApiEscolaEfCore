using ApiEscola.DTOs;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApiEscola.Entities
{
    public class RetornoAlunoDTO 
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Documento{ get; set; }
        public List<RetornoMateriaDTO> Materias { get; set; }
        
    }
}
