using System;
using System.Collections.Generic;

namespace ApiEscola.Entities
{
    public class Aluno : Pessoa
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Documento{ get; set; }
        public List<Materia> Materias { get; set; }
        
    }
}
