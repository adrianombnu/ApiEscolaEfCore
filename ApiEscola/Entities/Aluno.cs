using System;
using System.Collections.Generic;

namespace ApiEscola.Entities
{
    public class Aluno : Pessoa
    {
        public Aluno(string nome, string sobrenome, DateTime dataNascimento, string documento, List<Guid> idMaterias, Guid idTurma)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            DataNascimento = dataNascimento;
            Documento = documento;
            IdMaterias = idMaterias;
            IdTurma = idTurma;
            Id = Guid.NewGuid();
        }

        public Aluno(string nome, string sobrenome, DateTime dataNascimento, string documento, List<Guid> idMaterias, Guid idTurma, Guid id)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            DataNascimento = dataNascimento;
            Documento = documento;
            IdMaterias = idMaterias;
            IdTurma = idTurma;
            Id = id;
        }

        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Documento{ get; set; }
        public List<Guid> IdMaterias { get; set; }
        public Guid IdTurma { get; set; }

    }
}
