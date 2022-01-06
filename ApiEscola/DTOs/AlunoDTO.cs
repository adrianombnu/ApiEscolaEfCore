using ApiEscolaEfCore.DTOs;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApiEscolaEfCore.Entities
{
    public class AlunoDTO : Validator
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Documento{ get; set; }
        public List<Guid> IdMaterias { get; set; }
        
        public override void Validar()
        {
            Regex rgx = new Regex(@"[^a-zA-Z\s]");

            if (string.IsNullOrEmpty(Nome) )
                AddErros("Nome não informado");

            if (Nome.Length > 150)
                AddErros("Nome deve conter até 150 caracteres");

            if (rgx.IsMatch(Nome))
                AddErros("Nome contêm caracteres inválidos");

            if (string.IsNullOrEmpty(Sobrenome))
                AddErros("Sobrenome não informado");

            if (Sobrenome.Length > 150)
                AddErros("Sobrenome deve conter até 150 caracteres");

            if (rgx.IsMatch(Sobrenome))
                AddErros("Sobrenome contêm caracteres inválidos");

            rgx = new Regex("[^0-9]");

            if (string.IsNullOrEmpty(Documento))
                AddErros("Documento não informado");

            if (Documento.Length > 11)
                AddErros("Documento deve contêr até 11 caracteres");

            if (rgx.IsMatch(Documento))
                AddErros("Documento contêm caracteres inválidos");

            if (DataNascimento.Date > DateTime.Now.Date || string.IsNullOrEmpty(DataNascimento.ToString()))
                AddErros("Data de nascimento inválida");

            if (IdMaterias is null)
                AddErros("Nenhuma matéria foi informada.");

        }

    }
}
