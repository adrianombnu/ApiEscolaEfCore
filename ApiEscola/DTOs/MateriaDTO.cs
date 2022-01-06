using ApiEscolaEfCore.DTOs;
using System;
using System.Text.RegularExpressions;

namespace ApiEscolaEfCore.Entities
{
    public class MateriaDTO : Validator
    {
        public string Nome { get; set; }
        public Guid IdProfessor { get; set; }

        public override void Validar()
        {
            Valido = true;

            Regex rgx = new Regex(@"[^a-zA-Z\s]");

            if (rgx.IsMatch(Nome))
                AddErros("Nome da matéria contêm caracteres inválidos");

            if (string.IsNullOrEmpty(Nome))
                AddErros("Nome da matéria não foi informado.");

            if (Nome.Length > 150)
                AddErros("Nome da matéria somente pode conter até 150 caracteres.");

            if (string.IsNullOrEmpty(IdProfessor.ToString()))
                AddErros("Professor não informado.");

        }

    }
}
