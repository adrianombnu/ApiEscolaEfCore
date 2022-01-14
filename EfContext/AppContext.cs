﻿using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EfContext
{
    public class AppContext : DbContext
    {
        public DbSet<Materia> Materia { get; set; }
        public DbSet<Aluno> Aluno { get; set; }
        public DbSet<Professor> Professor { get; set; }
        public DbSet<Curso> Curso { get; set; }
        public DbSet<Turma> Turma { get; set; }

        public AppContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContext).Assembly);

            /*
            modelBuilder.Entity<Aluno>()
                        .Property(x => x.IdMaterias)
                        .HasConversion(new ValueConverter<List<Guid>, string>(
                            v => JsonConvert.SerializeObject(v), // Convert to string for persistence
                            v => JsonConvert.DeserializeObject<List<Guid>>(v)));

            modelBuilder.Entity<Turma>()
                        .Property(x => x.IdMaterias)
                        .HasConversion(new ValueConverter<List<Guid>, string>(
                            v => JsonConvert.SerializeObject(v), // Convert to string for persistence
                            v => JsonConvert.DeserializeObject<List<Guid>>(v)));

            
            */

            base.OnModelCreating(modelBuilder);
        }
    }


}

