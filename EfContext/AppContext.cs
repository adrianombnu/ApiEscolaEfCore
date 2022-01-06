using Dominio.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfContext
{
    public class AppContext : DbContext
    {
        public DbSet<Materia> Materias { get; set; }

        public AppContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }


}

