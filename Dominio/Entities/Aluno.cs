using System;
using System.Collections.Generic;

namespace Dominio.Entities
{
    public class Aluno : Pessoa
    {
        public Aluno() { }

        public Aluno(string nome, string sobrenome, DateTime dataNascimento, string documento, List<Guid> idMaterias)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            DataNascimento = dataNascimento;
            Documento = documento;
            IdMaterias = idMaterias;
            Id = Guid.NewGuid();
        }

        public Aluno(string nome, string sobrenome, DateTime dataNascimento, string documento, List<Guid> idMaterias, Guid id)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            DataNascimento = dataNascimento;
            Documento = documento;
            IdMaterias = idMaterias;
            Id = id;
        }

        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Documento{ get; set; }
        public List<Guid> IdMaterias { get; set; }

    }
}
