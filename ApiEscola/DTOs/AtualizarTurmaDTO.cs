﻿using System;

namespace ApiEscolaEfCore.DTOs
{
    public class AtualizarTurmaDTO : Validator
    {
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public Guid IdCurso { get; set; }


        public override void Validar()
        {
            Valido = true;

            if (string.IsNullOrEmpty(Nome))
                AddErros("Nome da turma não foi informada.");

            if(Nome.Length > 150)
                AddErros("Nome da turma somente pode conter até 150 caracteres.");

            if (string.IsNullOrEmpty(DataInicio.ToString()))
                AddErros("Data de inicio da turma não foi informada.");

            if (string.IsNullOrEmpty(DataFim.ToString()))
                AddErros("Data de fim da turma não foi informada.");

            if (IdCurso.ToString().Length <= 0 )
                AddErros("Curso não informado.");
           
        }
    }
}
