using System;
using System.Collections.Generic;

#nullable disable

namespace EfContext
{
    public partial class Professor
    {
        public Professor()
        {
            Materia = new HashSet<Materium>();
        }

        public string Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime Datadenascimento { get; set; }
        public string Documento { get; set; }

        public virtual ICollection<Materium> Materia { get; set; }
    }
}
