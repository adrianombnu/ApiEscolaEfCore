using System;

namespace EfContext
{
    public interface IUnitOfWork : IDisposable
    {
        IMateriaRepositoryEfCore MateriaRepository { get; }

        public bool Commit();
    }

}

