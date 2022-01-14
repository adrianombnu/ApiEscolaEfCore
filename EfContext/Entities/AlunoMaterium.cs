#nullable disable

namespace EfContext.Entities
{
    public partial class AlunoMaterium
    {
        public string Idaluno { get; set; }
        public string Idturmamateria { get; set; }

        public virtual Aluno IdalunoNavigation { get; set; }
        public virtual TurmaMaterium IdturmamateriaNavigation { get; set; }
    }
}
