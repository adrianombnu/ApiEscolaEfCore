using Dominio.Entities;
using System;
using System.Collections.Generic;

namespace Dominio
{
    public interface ICursoRepository : IRepositoryBase<Guid, Curso>
    {
        public bool VerificaSeCursoPossuiTurma(Guid idCurso);

        public IEnumerable<Curso> ListarCursos(string? nome = null, string? descricao = null, int page = 1, int itens = 50);
        public bool VerificaSeCursoJaCadastrado(string nomeCurso, Guid id);

        public bool BuscaCursoPeloNome(string nomeCurso);

    }
}