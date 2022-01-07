using Dominio.Entities;
using System;
using System.Collections.Generic;

namespace Dominio
{
    public interface ITurmaRepository : IRepositoryBase<Guid, Turma>
    {
        public bool BuscarTurmaPeloNome(string nomeTurma, Guid idTurma, bool consideraIdDifente = false);

        public bool VerificaVinculoMateriaComATurma(Guid idMateria, Guid idTurma);

        public bool VerificaSePossuiAlunoVinculado(Guid idTurma);

        public bool VerificaAlunoMatriculadoMateria(Guid idMateria, Guid idTurma);

        public int BuscarQuantidadeMateriasCadastradas(Guid idTurma);

        public IEnumerable<Turma> ListarTurmas(string? nome = null, DateTime? dataInicio = null, DateTime? dataFim = null, int page = 1, int itens = 50);
        
        public IEnumerable<Aluno> BuscarAlunos(Guid idTurma);

    }
}