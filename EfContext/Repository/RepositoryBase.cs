using EfContext.Entities;
using System;


namespace EfContext.Repository
{
    public class RepositoryBase<TEntity> : IRepositoryBaseEfCore<TEntity> where TEntity : class
    {
        private readonly ModelContext _context;

        public RepositoryBase(ModelContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Remover(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void Incluir(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Atualizar(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }
    }


}

