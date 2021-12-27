using System;

namespace ApiEscola.DTOs
{
    public class ResultadoDTO
    {
        public bool Sucesso { get; set; }
        public string[] Erros { get; set; }
        public Object Resultado { get; set; }

        public static ResultadoDTO ErroResultado(string menssageErro)
        {
            return new ResultadoDTO
            {
                Sucesso = false,
                Erros = new string[] { menssageErro },
                Resultado = null
            };
        }
        public static ResultadoDTO SucessoResultado(object resultado = null)
        {
            return new ResultadoDTO
            {
                Sucesso = true,
                Erros = null,
                Resultado = resultado
            };
        }

    }

}
