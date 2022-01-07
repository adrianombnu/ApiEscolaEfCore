using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace EfContext
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Aluno> Alunos { get; set; }
        public virtual DbSet<AlunoMaterium> AlunoMateria { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Curso> Cursos { get; set; }
        public virtual DbSet<Materium> Materia { get; set; }
        public virtual DbSet<Professor> Professors { get; set; }
        public virtual DbSet<Turma> Turmas { get; set; }
        public virtual DbSet<TurmaAluno> TurmaAlunos { get; set; }
        public virtual DbSet<TurmaMaterium> TurmaMateria { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseOracle("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XE)));User Id=AppAcademy;Password=AppAcademy;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("APPACADEMY");

            modelBuilder.Entity<Aluno>(entity =>
            {
                entity.ToTable("ALUNO");

                entity.HasIndex(e => e.Documento, "INDEX_ALUNO1")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.Datadenascimento)
                    .HasColumnType("DATE")
                    .HasColumnName("DATADENASCIMENTO");

                entity.Property(e => e.Documento)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENTO");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("NOME");

                entity.Property(e => e.Sobrenome)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("SOBRENOME");
            });

            modelBuilder.Entity<AlunoMaterium>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ALUNO_MATERIA");

                entity.HasIndex(e => new { e.Idaluno, e.Idturmamateria }, "INDEX_ALUNO_MATERIA1")
                    .IsUnique();

                entity.Property(e => e.Idaluno)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("IDALUNO");

                entity.Property(e => e.Idturmamateria)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("IDTURMAMATERIA");

                entity.HasOne(d => d.IdalunoNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idaluno)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ALUNO_MATERIA1");

                entity.HasOne(d => d.IdturmamateriaNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idturmamateria)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ALUNO_MATERIA2");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("clientes");

                entity.Property(e => e.ClienteId)
                    .HasMaxLength(36)
                    .HasColumnName("clienteId");

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Idade)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("idade");

                entity.Property(e => e.Nome)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("nome");
            });

            modelBuilder.Entity<Curso>(entity =>
            {
                entity.ToTable("CURSO");

                entity.HasIndex(e => e.Nome, "INDEX_CURSO1")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DESCRICAO");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("NOME");
            });

            modelBuilder.Entity<Materium>(entity =>
            {
                entity.ToTable("MATERIA");

                entity.HasIndex(e => new { e.Nome, e.Idprofessor }, "INDEX_MATERIA1")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.Idprofessor)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("IDPROFESSOR");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("NOME");

                entity.HasOne(d => d.IdprofessorNavigation)
                    .WithMany(p => p.Materia)
                    .HasForeignKey(d => d.Idprofessor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MATERIA1");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.ToTable("PROFESSOR");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.Datadenascimento)
                    .HasColumnType("DATE")
                    .HasColumnName("DATADENASCIMENTO");

                entity.Property(e => e.Documento)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENTO");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("NOME");

                entity.Property(e => e.Sobrenome)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("SOBRENOME");
            });

            modelBuilder.Entity<Turma>(entity =>
            {
                entity.ToTable("TURMA");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.Datafim)
                    .HasColumnType("DATE")
                    .HasColumnName("DATAFIM");

                entity.Property(e => e.Datainicio)
                    .HasColumnType("DATE")
                    .HasColumnName("DATAINICIO");

                entity.Property(e => e.Idcurso)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("IDCURSO");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("NOME");

                entity.HasOne(d => d.IdcursoNavigation)
                    .WithMany(p => p.Turmas)
                    .HasForeignKey(d => d.Idcurso)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TURMA1");
            });

            modelBuilder.Entity<TurmaAluno>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TURMA_ALUNO");

                entity.HasIndex(e => new { e.Idturma, e.Idaluno }, "INDEX_TURMA_ALUNO1")
                    .IsUnique();

                entity.Property(e => e.Idaluno)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("IDALUNO");

                entity.Property(e => e.Idturma)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("IDTURMA");

                entity.HasOne(d => d.IdalunoNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idaluno)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TURMA_ALUNO2");

                entity.HasOne(d => d.IdturmaNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idturma)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TURMA_ALUNO1");
            });

            modelBuilder.Entity<TurmaMaterium>(entity =>
            {
                entity.ToTable("TURMA_MATERIA");

                entity.HasIndex(e => new { e.Idturma, e.Idmateria }, "INDEX_TURMA_MATERIA1")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.Idmateria)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("IDMATERIA");

                entity.Property(e => e.Idturma)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("IDTURMA");

                entity.HasOne(d => d.IdmateriaNavigation)
                    .WithMany(p => p.TurmaMateria)
                    .HasForeignKey(d => d.Idmateria)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TURMA_MATERIA2");

                entity.HasOne(d => d.IdturmaNavigation)
                    .WithMany(p => p.TurmaMateria)
                    .HasForeignKey(d => d.Idturma)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TURMA_MATERIA1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
