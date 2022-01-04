using ApiEscola.Entities;
using ApiEscola.Extensions;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

#nullable enable
namespace ApiEscola.Repository
{
    public class TurmaRepository
    {
        private readonly IConfiguration _configuration;
        private List<Guid> _materiasCurso;
        private List<Turma> _turmas;
        private List<Aluno> _alunos;

        public TurmaRepository(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public bool Cadastrar(Turma turma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using OracleCommand command = conn.CreateCommand();
                OracleTransaction transaction;

                // Start a local transaction
                transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                // Assign transaction object for a pending local transaction
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"INSERT INTO APPACADEMY.turma
                                            (ID, NOME, DATAINICIO, DATAFIM, IDCURSO)
                                          VALUES(:Id,:Nome,:DataInicio,:DataFim, :IdCurso)";

                    command.Parameters.Add(new OracleParameter("Id", turma.Id.ToString()));
                    command.Parameters.Add(new OracleParameter("Nome", turma.Nome));
                    command.Parameters.Add(new OracleParameter("DataInicio", turma.DataInicio));
                    command.Parameters.Add(new OracleParameter("DataFim", turma.DataFim));
                    command.Parameters.Add(new OracleParameter("IdCurso", turma.IdCurso.ToString()));

                    var rows = command.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível cadastrar a turma.");

                    foreach (var id in turma.IdMaterias)
                    {
                        using OracleCommand commandAddMateria = conn.CreateCommand();
                        commandAddMateria.Transaction = transaction;

                        commandAddMateria.CommandText = @"INSERT INTO APPACADEMY.turma_materia
                                                            (ID, IDTURMA, IDMATERIA)
                                                          VALUES(:Id, :IdTurma, :IdMateria)";

                        commandAddMateria.Parameters.Add(new OracleParameter("Id", Guid.NewGuid().ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdTurma", turma.Id.ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdMateria", id.ToString()));

                        rows = commandAddMateria.ExecuteNonQuery();

                        if (rows == 0)
                            throw new Exception("Não foi possível cadastrar a turma.");

                    }

                    transaction.Commit();
                    retorno = true;

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    retorno = false;
                }

            }

            return retorno;

        }

        public bool AtualizarTurma(Turma turma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(
                    @"UPDATE APPACADEMY.turma
                        SET ID = :Id,
                            NOME = :Nome, 
                            DATAINICIO = :DataInicio,
                            DATAFIM = :DataFim,
                            IDCURSO = :IdCurso
                      WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", turma.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", turma.Nome));
                cmd.Parameters.Add(new OracleParameter("DataInicio", turma.DataInicio));
                cmd.Parameters.Add(new OracleParameter("DataFim", turma.DataFim));
                cmd.Parameters.Add(new OracleParameter("IdCurso", turma.IdCurso.ToString()));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool RomoverTurma(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using OracleCommand command = conn.CreateCommand();
                OracleTransaction transaction;

                // Start a local transaction
                transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                // Assign transaction object for a pending local transaction
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"DELETE FROM APPACADEMY.turma_materia
                                                  WHERE IDTURMA = :IdTurma ";

                    command.Parameters.Add(new OracleParameter("Id", id.ToString()));

                    var rows = command.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível remover as matérias vinculadas a turma.");

                    using OracleCommand commandAddMateria = conn.CreateCommand();
                    commandAddMateria.Transaction = transaction;

                    commandAddMateria.CommandText = @"DELETE FROM APPACADEMY.turma
                                                            WHERE ID = :IdTurma ";

                    commandAddMateria.Parameters.Add(new OracleParameter("IdTurma", id.ToString()));

                    rows = commandAddMateria.ExecuteNonQuery();

                    if (rows == 0)
                        throw new Exception("Não foi possível remover a turma.");


                    transaction.Commit();
                    retorno = true;

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    retorno = false;
                }

            }

            return retorno;
        }

        public bool AdicionarMaterias(Guid idTurma, List<Guid> idMaterias)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using OracleCommand command = conn.CreateCommand();
                OracleTransaction transaction;

                // Start a local transaction
                transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                // Assign transaction object for a pending local transaction
                command.Transaction = transaction;

                try
                {
                    foreach (var id in idMaterias)
                    {
                        using OracleCommand commandAddMateria = conn.CreateCommand();
                        commandAddMateria.Transaction = transaction;

                        commandAddMateria.CommandText = @"INSERT INTO APPACADEMY.turma_materia
                                                            (ID, IDTURMA, IDMATERIA)
                                                          VALUES(:Id, :IdTurma, :IdMateria)";

                        commandAddMateria.Parameters.Add(new OracleParameter("Id", Guid.NewGuid().ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdMateria", id.ToString()));

                        var rows = commandAddMateria.ExecuteNonQuery();

                        if (rows == 0)
                            throw new Exception("Não foi possível adicionar a matéria.");

                    }

                    transaction.Commit();
                    retorno = true;

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    retorno = false;
                }

            }

            return retorno;

        }

        public bool RemoverMaterias(Guid idTurma, List<Guid> idMaterias)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using OracleCommand command = conn.CreateCommand();
                OracleTransaction transaction;

                // Start a local transaction
                transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                // Assign transaction object for a pending local transaction
                command.Transaction = transaction;

                try
                {
                    foreach (var id in idMaterias)
                    {
                        using OracleCommand commandAddMateria = conn.CreateCommand();
                        commandAddMateria.Transaction = transaction;

                        commandAddMateria.CommandText = @"DELETE FROM APPACADEMY.turma_materia
                                                                WHERE IDTURMA = :IdTurma
                                                                  AND IDMATERIA = :IdMateria";

                        commandAddMateria.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                        commandAddMateria.Parameters.Add(new OracleParameter("IdMateria", id.ToString()));

                        var rows = commandAddMateria.ExecuteNonQuery();

                        if (rows == 0)
                            throw new Exception("Não foi possível remover a matéria.");

                    }

                    transaction.Commit();
                    retorno = true;

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    retorno = false;
                }

            }

            return retorno;

        }

        public bool BuscarTurmaPeloNome(string nomeTurma, Guid idTurma, bool consideraIdDifente = false)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var cursoEncontrado = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                var query = (@"SELECT * FROM TURMA T WHERE 1 = 1");

                var sb = new StringBuilder(query);

                sb.Append(" AND UPPER(T.NOME) = :Nome ");

                if (consideraIdDifente)
                    sb.Append(" AND T.ID <> :IdTurma ");

                using var cmdTurma = new OracleCommand(sb.ToString(), conn);

                //Esse bind serve para que quando, for passado mais parametros do que o necessário para montar o comando sql, devido a ser criado de forma dinamica, vamos evitar que dê
                //problema de quantidade maior ou a menor
                cmdTurma.BindByName = true;

                cmdTurma.Parameters.Add(new OracleParameter("Nome", nomeTurma.ToUpperIgnoreNull()));
                cmdTurma.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));

                using var reader = cmdTurma.ExecuteReader();

                if (reader.HasRows)
                    cursoEncontrado = true;

            }

            return cursoEncontrado;

        }

        public bool VerificaVinculoMateriaComATurma(Guid idMateria, Guid idTurma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var materiaVinculada = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                var query = (@"SELECT * FROM TURMA_MATERIA TM
                                        WHERE TM.IDTURMA = :IdTurma 
                                          AND TM.IDMATERIA = :IdMateria ");

                using var cmd = new OracleCommand(query, conn);

                cmd.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                cmd.Parameters.Add(new OracleParameter("IdMateria", idMateria.ToString()));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    materiaVinculada = true;

            }

            return materiaVinculada;

        }

        public bool VerificaSePossuiAlunoVinculado(Guid idTurma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT COUNT(ID) QUANTIDADE
                                                                     FROM turma_materia tm
                                                               INNER JOIN ALUNO_MATERIA am
                                                                       ON tm.ID = am.IDTURMAMATERIA
                                                                    WHERE tm.IDTURMA = :IdTurma ", conn);

                cmd.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["quantidade"]) > 0)
                        retorno = true;
                }

            }

            return retorno;

        }

        public bool VerificaAlunoMatriculadoMateria(Guid idMateria, Guid idTurma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT COUNT(ID) QUANTIDADE
                                                                     FROM turma_materia tm
                                                               INNER JOIN ALUNO_MATERIA am
                                                                       ON tm.ID = am.IDTURMAMATERIA
                                                                    WHERE tm.IDTURMA = :IdTurma
                                                                      AND IDMATERIA = :IdMateria ", conn);

                cmd.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                cmd.Parameters.Add(new OracleParameter("IdMateria", idMateria.ToString()));

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["quantidade"]) > 0)
                        retorno = true;
                }

            }

            return retorno;

        }

        public Turma BuscarTurmaPeloId(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM TURMA WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", id.ToString()));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //buscar materias da turma
                        using var cmdMateriasCurso = new OracleCommand(@"SELECT * FROM materia a 
                                                                                 WHERE a.ID IN (SELECT tm.IDMATERIA 
                                                                                                  FROM turma_materia tm 
                                                                                                 WHERE tm.IDTURMA = :IdTurma)", conn);

                        cmdMateriasCurso.Parameters.Add(new OracleParameter("IdTurma", reader["id"].ToString()));

                        using (var readerMateriasCurso = cmdMateriasCurso.ExecuteReader())
                        {
                            _materiasCurso = new List<Guid>();

                            while (readerMateriasCurso.Read())
                            {
                                _materiasCurso.Add(Guid.Parse(Convert.ToString(readerMateriasCurso["id"])));

                            }
                        }

                        return new Turma(Convert.ToString(reader["nome"]), Convert.ToDateTime(reader["dataInicio"]), Convert.ToDateTime(reader["dataFim"]), _materiasCurso, Guid.Parse(Convert.ToString(reader["id"])), Guid.Parse(Convert.ToString(reader["idCurso"])));

                    }
                }

            }

            return null;

        }

        public int BuscarQuantidadeMateriasCadastradas(Guid idTurma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var quantidade = 0;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT COUNT(ID) QUANTIDADE 
                                                      FROM TURMA_MATERIA 
                                                     WHERE IDTURMA = :IdTurma", conn);

                cmd.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    quantidade = Convert.ToInt32(reader["quantidade"]);
                }

            }

            return quantidade;

        }

        public IEnumerable<Aluno> BuscarAlunos(Guid idTurma)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            
            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT DISTINCT am.IDALUNO ,
                                                                    AL.* 
                                                      FROM turma a
                                                     inner JOIN TURMA_MATERIA tm
                                                            ON a.ID = tm.IDTURMA
                                                     inner JOIN materia M
                                                            ON M.ID = tm.IDMATERIA
                                                     inner JOIN ALUNO_MATERIA am
                                                            ON am.IDTURMAMATERIA = tm.ID
                                                     inner JOIN aluno AL
                                                           ON AL.ID = am.IDALUNO
                                                     WHERE a.ID = :IdTurma", conn);

                cmd.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    _alunos ??= new List<Aluno>();

                    //buscar materias da turma
                    using var cmdMateriasCurso = new OracleCommand(@"SELECT tm.IDMATERIA
                                                                      FROM turma a
                                                                     inner JOIN TURMA_MATERIA tm 
                                                                            ON a.ID = tm.IDTURMA
                                                                     inner JOIN ALUNO_MATERIA am 
                                                                            ON am.IDTURMAMATERIA = tm.ID
                                                                     inner JOIN aluno AL 
	                                                                       ON AL.ID = am.IDALUNO	 
	                                                                 WHERE tm.IDTURMA = :IdTurma
                                                                       AND AL.ID = :IdAluno", conn);

                    cmdMateriasCurso.Parameters.Add(new OracleParameter("IdTurma", idTurma.ToString()));
                    cmdMateriasCurso.Parameters.Add(new OracleParameter("IdAluno", reader["ID"].ToString()));

                    _materiasCurso = new List<Guid>();

                    using (var readerMateriasCurso = cmdMateriasCurso.ExecuteReader())
                    {
                        while (readerMateriasCurso.Read())
                        {
                            _materiasCurso.Add(Guid.Parse(Convert.ToString(readerMateriasCurso["IDMATERIA"])));

                        }
                    }

                    var aluno = new Aluno(Convert.ToString(reader["nome"]), Convert.ToString(reader["sobrenome"]), Convert.ToDateTime(reader["dataDeNascimento"]), Convert.ToString(reader["documento"]), _materiasCurso, Guid.Parse(Convert.ToString(reader["id"])));

                    _alunos.Add(aluno);
                }

            }

            return _alunos;

        }

        public IEnumerable<Turma> ListarTurmas(string? nome = null, DateTime? dataInicio = null, DateTime? dataFim = null, int page = 1, int itens = 50)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                var query = (@"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ROWID) AS RN,
                                                     T.* 
                                                FROM TURMA T 
                                               WHERE 1 = 1");

                var sb = new StringBuilder(query);

                if (!string.IsNullOrEmpty(nome))
                    sb.Append(" AND UPPER(T.NOME) LIKE '%' || :Nome || '%'");

                if (!string.IsNullOrEmpty(dataInicio.ToString()))
                    sb.Append(" AND to_char(T.DATAINICIO,'dd/mm/rrrr') = :DataInicio");

                if (!string.IsNullOrEmpty(dataFim.ToString()))
                    sb.Append(" AND to_char(T.DATAFIM,'dd/mm/rrrr') = :DataFim");

                sb.Append(" ) TURMAS");
                sb.Append(" WHERE ROWNUM <= :Itens AND TURMAS.RN > (:Page -1) * :Itens");

                using var cmdTurma = new OracleCommand(sb.ToString(), conn);

                //Esse bind serve para que quando, for passado mais parametros do que o necessário para montar o comando sql, devido a ser criado de forma dinamica, vamos evitar que dê
                //problema de quantidade maior ou a menor
                cmdTurma.BindByName = true;

                cmdTurma.Parameters.Add(new OracleParameter("Nome", nome.ToUpperIgnoreNull()));
                cmdTurma.Parameters.Add(new OracleParameter("DataInicio", (dataInicio.HasValue ? dataInicio.Value.ToString("dd/MM/yyyy") : "null")));
                cmdTurma.Parameters.Add(new OracleParameter("DataFim", (dataFim.HasValue ? dataFim.Value.ToString("dd/MM/yyyy") : "null")));
                cmdTurma.Parameters.Add(new OracleParameter("Itens", itens));
                cmdTurma.Parameters.Add(new OracleParameter("Page", page));

                using (var readerTurma = cmdTurma.ExecuteReader())
                {
                    _turmas = new List<Turma>();

                    while (readerTurma.Read())
                    {
                        //buscar materias da turma
                        using var cmdMateriasCurso = new OracleCommand(@"SELECT tm.IDMATERIA 
                                                                           FROM turma_materia tm 
                                                                          WHERE tm.IDTURMA = :IdTurma", conn);

                        cmdMateriasCurso.Parameters.Add(new OracleParameter("IdTurma", readerTurma["id"].ToString()));

                        using (var readerMateriasCurso = cmdMateriasCurso.ExecuteReader())
                        {
                            _materiasCurso = new List<Guid>();

                            while (readerMateriasCurso.Read())
                            {
                                _materiasCurso.Add(Guid.Parse(Convert.ToString(readerMateriasCurso["idmateria"])));

                            }
                        }

                        var turma = new Turma(Convert.ToString(readerTurma["nome"]), Convert.ToDateTime(readerTurma["dataInicio"]), Convert.ToDateTime(readerTurma["dataFim"]), _materiasCurso, Guid.Parse(Convert.ToString(readerTurma["id"])), Guid.Parse(Convert.ToString(readerTurma["idCurso"])));

                        _turmas.Add(turma);

                    }

                }

            }

            //return _turmas.Skip((page - 1) * itens).Take(itens);
            return _turmas;
        }

    }

}
