using System;
using System.Collections.Generic;

#nullable disable

namespace EfContext.Entities
{
    public partial class Turma
    {
        public Turma()
        {
            TurmaMateria = new HashSet<TurmaMaterium>();
        }

        public string Id { get; set; }
        public string Nome { get; set; }
        public DateTime Datainicio { get; set; }
        public DateTime Datafim { get; set; }
        public string Idcurso { get; set; }

        public virtual Curso IdcursoNavigation { get; set; }
        public virtual ICollection<TurmaMaterium> TurmaMateria { get; set; }
    }
}
