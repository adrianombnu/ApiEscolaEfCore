using System;

namespace Dominio.Entities
{
    public class Professor : Pessoa
    {
        public Professor() { }

        public Professor(string nome, string sobrenome, DateTime dataNascimento, string documento)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            DataNascimento = dataNascimento;
            Documento = documento;
            Id = Guid.NewGuid();
        }

        public Professor(string nome, string sobrenome, DateTime dataNascimento, string documento, Guid id)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            DataNascimento = dataNascimento;
            Documento = documento;
            Id = id;
        }

        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Documento { get; set; }

        
        
    }
}
