using System.Collections.Generic;

#nullable disable

namespace EfContext.Entities
{
    public partial class Curso
    {
        public Curso()
        {
            Turmas = new HashSet<Turma>();
        }

        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public virtual ICollection<Turma> Turmas { get; set; }
    }
}
