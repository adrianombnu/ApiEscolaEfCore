using Dominio;
using Dominio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EfContext.Repository
{
    public class RepositoryBase<TKey, TEntity> : IRepositoryBaseEfCore<TKey, TEntity> where TEntity : Base<TKey>
    {
        private readonly AppContext _context;

        public RepositoryBase(AppContext context)
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

