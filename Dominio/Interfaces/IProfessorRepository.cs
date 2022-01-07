using Dominio.Entities;
using System;
using System.Collections.Generic;

namespace Dominio
{
    public interface IProfessorRepository : IRepositoryBase<Guid, Professor>
    {
        public bool BuscaProfessorPeloDocumento(string documento);

        public bool VerificaSePossuiMateriaVinculada(Guid id);

        public bool VerificaSeProfessorJaCadastrado(string documento, Guid id);

        public IEnumerable<Professor> ListarProfessores(string? nome = null, string? sobrenome = null, DateTime? dataDeNascimento = null, string? documento = null, int page = 1, int itens = 50);
    }
}