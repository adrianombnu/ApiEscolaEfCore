using System;

#nullable disable

namespace EfContext.Entities
{
    public partial class Aluno
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime Datadenascimento { get; set; }
        public string Documento { get; set; }
    }
}
