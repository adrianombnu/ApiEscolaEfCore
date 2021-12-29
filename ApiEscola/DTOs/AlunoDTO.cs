﻿using ApiEscola.DTOs;
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
        
        public override void Validar()
        {
            Regex rgx = new Regex(@"[^a-zA-Z\s]");

            if (string.IsNullOrEmpty(Nome) || Nome.Length > 150 || rgx.IsMatch(Nome))
                AddErros("Nome inválido");

            if (string.IsNullOrEmpty(Sobrenome) || Sobrenome.Length > 150 || rgx.IsMatch(Sobrenome))
                AddErros("Sobrenome inválido");

            rgx = new Regex("[^0-9]");

            if (rgx.IsMatch(Documento))
                AddErros("Documento inválido");

            if (DataNascimento.Date > DateTime.Now.Date || string.IsNullOrEmpty(DataNascimento.ToString()))
                AddErros("Data de nascimento inválida");

            if (IdMaterias is null)
                AddErros("Nenhuma matéria foi informada.");

        }

    }
}
