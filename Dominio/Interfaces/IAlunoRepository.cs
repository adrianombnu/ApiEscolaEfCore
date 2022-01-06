using Dominio.Entities;
using System;
using System.Collections.Generic;

namespace Dominio
{
    public interface IAlunoRepository : IRepositoryBase<Guid, Aluno>
    {
        public bool VerificaSeAlunoJaCadastrado(string documento, Guid idTurma);

        public IEnumerable<Aluno> ListarAlunos(string? nome = null, string? sobrenome = null, DateTime? dataDeNascimento = null, int page = 1, int itens = 50);

    }
}