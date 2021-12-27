namespace ApiEscola.DTOs
{
    public class CursoDTO : Validator
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public override void Validar()
        {
            Valido = true;

            if (string.IsNullOrEmpty(Nome))
                AddErros("Nome do curso não foi informado.");

            if(Nome.Length > 150)
                AddErros("Nome do curso somente pode conter até 150 caracteres.");

            if (string.IsNullOrEmpty(Descricao))
                AddErros("Descrição do curso não foi informado.");
                    
            if(Descricao.Length > 500)
                AddErros("Descrição do curso somente pode conter até 500 caracteres.");

        }
    }
}
