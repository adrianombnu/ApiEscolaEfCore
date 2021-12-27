using System.Text.RegularExpressions;

namespace ApiEscola.DTOs
{
    public class CursoDTO : Validator
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public override void Validar()
        {
            Valido = true;

            Regex rgx = new Regex(@"[^a-zA-Z\s]");

            if (rgx.IsMatch(Nome))
                AddErros("Nome do curso contêm caracteres inválidos");

            if (string.IsNullOrEmpty(Nome))
                AddErros("Nome do curso não foi informado.");

            if(Nome.Length > 150)
                AddErros("Nome do curso somente pode conter até 150 caracteres.");

            if (rgx.IsMatch(Descricao))
                AddErros("Descrição do curso contêm caracteres inválidos");

            if (string.IsNullOrEmpty(Descricao))
                AddErros("Descrição do curso não foi informado.");
                    
            if(Descricao.Length > 500)
                AddErros("Descrição do curso somente pode conter até 500 caracteres.");

        }
    }
}
