using Dominio.Entities;

namespace Dominio
{
    public interface IRepositoryBaseEfCore<TKey, TEntity> where TEntity : Base<TKey>
    {
        public void Incluir(TEntity entity);
        public void Atualizar(TEntity entity);
        public void Remover(TEntity entity);
    }

}