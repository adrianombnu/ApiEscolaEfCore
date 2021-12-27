using ApiEscola.DTOs;
using System;
using System.Text.RegularExpressions;

namespace ApiEscola.Entities
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
                AddErros("Nome da materia contêm caracteres inválidos");

            if (string.IsNullOrEmpty(Nome))
                AddErros("Nome da materia não foi informado.");

            if (Nome.Length > 150)
                AddErros("Nome da materia somente pode conter até 150 caracteres.");

            if (string.IsNullOrEmpty(IdProfessor.ToString()))
                AddErros("Professor não informado.");

        }

    }
}
