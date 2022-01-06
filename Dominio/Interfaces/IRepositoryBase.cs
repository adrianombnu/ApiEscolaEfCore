using Dominio.Entities;
using System.Collections.Generic;

namespace Dominio
{
    public interface IRepositoryBase<TKey, TEntity> where TEntity : Base<TKey>
    {
        public TEntity BuscarPeloId(TKey id);
        
    }

}