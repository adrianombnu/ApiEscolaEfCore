using System;
using System.Collections.Generic;

namespace ApiEscola.Entities
{
    public class Aluno : Pessoa
    {
        public Aluno(string nome, string sobrenome, DateTime dataNascimento, string documento, List<Materia> materias)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            DataNascimento = dataNascimento;
            Documento = documento;
            Materias = materias;
            Id = Guid.NewGuid();
        }

        public Aluno(string nome, string sobrenome, DateTime dataNascimento, string documento, List<Materia> materias, Guid id)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            DataNascimento = dataNascimento;
            Documento = documento;
            Materias = materias;
            Id = id;
        }

        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Documento{ get; set; }
        public List<Materia> Materias { get; set; }
        
    }
}
