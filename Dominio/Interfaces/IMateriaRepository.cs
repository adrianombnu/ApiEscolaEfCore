using Dominio.Entities;
using System;
using System.Collections.Generic;

namespace Dominio
{
    public interface IMateriaRepository : IRepositoryBase<Guid, Materia>
    {
        public bool VerificaSePossuiTurmaVinculada(Guid id);
        
        public bool VerificaSeMateriaJaCadastrada(string nomeMateria, Guid idMateria, Guid idProfessor, bool consideraIdDifente = false);

        public IEnumerable<Materia> ListarMaterias(string? nome = null, int page = 1, int itens = 50);

    }
}