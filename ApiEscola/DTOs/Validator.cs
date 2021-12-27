using System.Collections.Generic;

namespace ApiEscola.DTOs
{
    public abstract class Validator
    {
        public bool Valido { get; protected set; }

        public List<string> Erros { get; protected set; }

        public Validator()
        {
            Valido = true;
            Erros ??= new List<string>();
        }

        public void AddErros(string descricaoErro)
        {
            Valido = false;
            Erros.Add(descricaoErro);
        }

        public abstract void Validar();
    }
}
