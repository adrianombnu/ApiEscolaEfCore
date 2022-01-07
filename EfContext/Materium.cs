using System;
using System.Collections.Generic;

#nullable disable

namespace EfContext
{
    public partial class Materium
    {
        public Materium()
        {
            TurmaMateria = new HashSet<TurmaMaterium>();
        }

        public string Id { get; set; }
        public string Nome { get; set; }
        public string Idprofessor { get; set; }

        public virtual Professor IdprofessorNavigation { get; set; }
        public virtual ICollection<TurmaMaterium> TurmaMateria { get; set; }
    }
}
