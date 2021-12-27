using System;

namespace ApiEscola.Entities
{
    public class Materia : Base
    {
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
