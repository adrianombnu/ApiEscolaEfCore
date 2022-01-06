using System;

namespace Dominio.Entities
{
    public class Materia : Base<Guid>
    {
        public Materia() { }

        public Materia(string nome, Guid idProfessor)
        {
            Nome = nome;
            IdProfessor = idProfessor;
            Id = Guid.NewGuid();
        }

        public Materia(string nome, Guid idProfessor, Guid id)
        {
            Nome = nome;
            IdProfessor = idProfessor;
            Id = id;
        }

        public string Nome { get; set; }
        public Guid IdProfessor { get; set; }
        
    }
}
