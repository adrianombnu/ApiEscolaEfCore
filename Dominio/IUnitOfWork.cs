using System;

namespace Dominio
{
    public interface IUnitOfWork : IDisposable
    {
        IMateriaRepositoryEfCore MateriaRepository { get; }

        public bool Commit();
    }

}

