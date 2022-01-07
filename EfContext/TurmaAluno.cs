using System;
using System.Collections.Generic;

#nullable disable

namespace EfContext
{
    public partial class TurmaAluno
    {
        public string Idturma { get; set; }
        public string Idaluno { get; set; }

        public virtual Aluno IdalunoNavigation { get; set; }
        public virtual Turma IdturmaNavigation { get; set; }
    }
}
