using System;

namespace Dominio
{
    public interface IUnitOfWork : IDisposable
    {
        IMateriaRepository MateriaRepository { get; }

        public bool Commit();
    }

}

