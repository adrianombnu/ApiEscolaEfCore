using System;

namespace Dominio.Entities
{
    public class Curso : Base<Guid>
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public Curso(string nome, string descricao)
        {
            Nome = nome;
            Descricao = descricao;
            Id = Guid.NewGuid();    
        }
        
        public Curso(string nome, string descricao, Guid id)
        {
            Nome = nome;
            Descricao = descricao;
            Id = id;
        }

    }
}
