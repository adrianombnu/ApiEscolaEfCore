using System;
using System.Text.RegularExpressions;

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

            Regex rgx = new Regex(@"[^a-zA-Z\s]");

            if (rgx.IsMatch(Nome))
                AddErros("Nome contêm caracteres inválidos");

            if (string.IsNullOrEmpty(Nome))
                AddErros("Nome não foi informado.");

            if (Nome.Length > 150)
                AddErros("Nome somente pode conter até 150 caracteres.");

            if (rgx.IsMatch(Sobrenome))
                AddErros("Sobrenome contêm caracteres inválidos");

            if (string.IsNullOrEmpty(Sobrenome))
                AddErros("Sobrenome não foi informado.");

            if (Sobrenome.Length > 150)
                AddErros("Sobrenome somente pode conter até 150 caracteres.");

            rgx = new Regex("[^0-9]");

            if (rgx.IsMatch(Documento))
                AddErros("Documento contêm caracteres inválidos");

            if (string.IsNullOrEmpty(Documento))
                AddErros("Data de nascimento não foi informada.");

            if (Documento.Length > 11)
                AddErros("Documento somente pode conter até 11 caracteres.");

        }
    }
}
