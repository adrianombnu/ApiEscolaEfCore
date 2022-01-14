using EfContext.Entities;

namespace EfContext
{
    public interface IRepositoryBaseEfCore<TEntity> //where TEntity : Base<TKey>
    {
        public void Incluir(TEntity entity);
        public void Atualizar(TEntity entity);
        public void Remover(TEntity entity);
    }

}