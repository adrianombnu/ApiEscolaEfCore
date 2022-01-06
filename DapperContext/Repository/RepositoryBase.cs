using Dominio;
using Dominio.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;


namespace DapperContext.Repository
{
    public abstract class RepositoryBase<TKey, TEntity> : IRepositoryBase<TKey, TEntity> where TEntity : Base<TKey>
    {
        protected OracleTransaction? _transaction;
        protected readonly OracleConnection _connection;
        
        public RepositoryBase(IConfiguration configuration)
        {
            _connection = new OracleConnection(configuration.GetConnectionString("Conexao"));
                        
        }

        public abstract TEntity BuscarPeloId(TKey id);
        
        public void BeginTransaction()
        {
            _connection.Open();
            _transaction = _connection?.BeginTransaction();
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }
    }

}