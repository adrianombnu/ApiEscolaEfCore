
#nullable disable

namespace EfContext.Entities
{
    public partial class TurmaMaterium
    {
        public string Id { get; set; }
        public string Idturma { get; set; }
        public string Idmateria { get; set; }

        public virtual Materium IdmateriaNavigation { get; set; }
        public virtual Turma IdturmaNavigation { get; set; }
    }
}
