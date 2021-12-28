using ApiEscola.DTOs;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApiEscola.Entities
{
    public class AlunoDTO : Validator
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Documento{ get; set; }
        public List<Guid> IdMaterias { get; set; }
        public Guid IdTurma { get; set; }

        public override void Validar()
        {
            Regex rgx = new Regex(@"[^a-zA-Z\s]");

            if (string.IsNullOrEmpty(Nome) || Nome.Length > 150 || rgx.IsMatch(Nome))
                AddErros("Nome invalido");

            if (string.IsNullOrEmpty(Sobrenome) || Sobrenome.Length > 150 || rgx.IsMatch(Sobrenome))
                AddErros("Sobrenome invalido");

            rgx = new Regex("[^0-9]");

            if (rgx.IsMatch(Documento))
                AddErros("Documento invalido");

            if (DataNascimento.Date > DateTime.Now.Date || string.IsNullOrEmpty(DataNascimento.ToString()))
                AddErros("Data de nascimento invalida");

            if (IdTurma.ToString().Length <= 0)
                AddErros("Turma não informada.");

            if (IdMaterias is null)
                AddErros("Nenhuma materia foi informada.");

        }

    }
}
