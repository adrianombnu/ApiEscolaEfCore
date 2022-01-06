using ApiEscolaEfCore.Entities;
using System;
using System.Collections.Generic;

namespace ApiEscolaEfCore.DTOs
{
    public class TurmaMateriaDTO : Validator
    {
        public List<Guid> IdMaterias { get; set; }

        public override void Validar()
        {
            Valido = true;

            if (IdMaterias is null)
                AddErros("Nenhuma materia foi informada.");

        }
    }
}
