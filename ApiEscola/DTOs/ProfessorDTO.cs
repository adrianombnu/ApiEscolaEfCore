using System;

namespace ApiEscola.DTOs
{
    public class ProfessorDTO : Validator
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Documento { get; set; }

        public override void Validar()
        {
            Valido = true;

            if (string.IsNullOrEmpty(Nome))
                AddErros("Nome do professor não foi informado.");

            if (Nome.Length > 150)
                AddErros("Nome do professor somente pode conter até 150 caracteres.");

            if (string.IsNullOrEmpty(Sobrenome))
                AddErros("Sobrenome do professor não foi informado.");

            if (Sobrenome.Length > 150)
                AddErros("Sobrenome do professor somente pode conter até 150 caracteres.");

            if (string.IsNullOrEmpty(Documento))
                AddErros("Data de nascimento não foi informada.");

            if (Documento.Length > 11)
                AddErros("Documento somente pode conter até 11 caracteres.");

        }
    }
}
