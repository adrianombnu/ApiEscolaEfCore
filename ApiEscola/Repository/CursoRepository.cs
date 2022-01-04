using ApiEscola.Entities;
using ApiEscola.Extensions;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace ApiEscola.Repository
{

    public class CursoRepository
    {
        private readonly IConfiguration _configuration;
        private readonly List<Curso> _cursos;

        public CursoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _cursos ??= new List<Curso>();

        }

        public bool Cadastrar(Curso curso)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"INSERT INTO APPACADEMY.CURSO
                                                    (ID, NOME, DESCRICAO)
                                                  VALUES(:Id,:Nome,:Descricao)", conn);

                cmd.Parameters.Add(new OracleParameter("Id", curso.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", curso.Nome));
                cmd.Parameters.Add(new OracleParameter("Descricao", curso.Descricao));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool AtualizarCurso(Curso curso)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"UPDATE APPACADEMY.CURSO
                                                       SET ID = :Id,
                                                           NOME = :Nome, 
                                                           DESCRICAO = :Descricao
                                                     WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", curso.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", curso.Nome));
                cmd.Parameters.Add(new OracleParameter("Descricao", curso.Descricao));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool RomoveCurso(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"DELETE FROM APPACADEMY.CURSO                        
                                                          WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", id.ToString()));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool BuscaCursoPeloNome(string nomeCurso)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var cursoEncontrado = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM CURSO 
                                                            WHERE UPPER(NOME) = :Nome", conn);

                cmd.Parameters.Add(new OracleParameter("Nome", nomeCurso.ToUpperIgnoreNull()));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    cursoEncontrado = true;

            }

            return cursoEncontrado;

        }

        public bool VerificaSeCursoJaCadastrado(string nomeCurso, Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var cursoEncontrado = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM CURSO 
                                                            WHERE UPPER(NOME) = :Nome 
                                                              AND ID <> :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Nome", nomeCurso.ToUpperIgnoreNull()));
                cmd.Parameters.Add(new OracleParameter("Id", id.ToString()));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    cursoEncontrado = true;

            }

            return cursoEncontrado;

        }

        public Curso BuscarCursoPeloId(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM CURSO WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", id.ToString()));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Curso(Convert.ToString(reader["nome"]), Convert.ToString(reader["descricao"]), Guid.Parse(Convert.ToString(reader["id"])));

                    }
                }

            }

            return null;

        }

        public bool VerificaSeCursoPossuiTurma(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM CURSO C 
                                                       INNER JOIN TURMA T 
                                                               ON C.ID = T.IDCURSO 
                                                            WHERE C.ID  = :IdCurso", conn);

                cmd.Parameters.Add(new OracleParameter("IdCurso", id.ToString()));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    return true;

            }

            return false;

        }

        public IEnumerable<Curso> ListarCursos(string? nome = null, string? descricao = null, int page = 1, int itens = 50)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                /*
                using var cmd = new OracleCommand();

                var query = (@"SELECT * FROM (SELECT ROWNUM AS RN, C.* FROM CURSO C WHERE 1 = 1");

                var sb = new StringBuilder(query);

                if (!string.IsNullOrEmpty(nome))
                {
                    sb.Append(" AND UPPER(C.NOME) LIKE '%' || :Nome || '%'");
                    cmd.Parameters.Add(new OracleParameter("Nome", nome.ToUpperIgnoreNull()));
                }

                if (!string.IsNullOrEmpty(descricao))
                {
                    sb.Append(" AND UPPER(C.DESCRICAO) LIKE '%' || :Descricao || '%'");
                    cmd.Parameters.Add(new OracleParameter("Descricao", descricao.ToUpperIgnoreNull()));
                }

                sb.Append(" ORDER BY ROWNUM) CURSOS");
                sb.Append(" WHERE ROWNUM <= :Itens AND CURSOS.RN > (:Page -1) * :Itens");
                cmd.Parameters.Add(new OracleParameter("Itens", itens));
                cmd.Parameters.Add(new OracleParameter("Page", page));

                cmd.Connection = conn;
                cmd.CommandText = sb.ToString();
                */

                var query = (@"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ROWID) AS RN,
                                                     C.* 
                                                FROM CURSO C 
                                               WHERE 1 = 1");

                var sb = new StringBuilder(query);

                if (!string.IsNullOrEmpty(nome))
                    sb.Append(" AND UPPER(C.NOME) LIKE '%' || :Nome || '%'");

                if (!string.IsNullOrEmpty(descricao))
                    sb.Append(" AND UPPER(C.DESCRICAO) LIKE '%' || :Descricao || '%'");

                sb.Append(" ) CURSOS");
                sb.Append(" WHERE ROWNUM <= :Itens AND CURSOS.RN > (:Page -1) * :Itens");

                using var cmd = new OracleCommand(sb.ToString(), conn);

                //Esse bind serve para que quando, for passado mais parametros do que o necessário para montar o comando sql, devido a ser criado de forma dinamica, vamos evitar que dê
                //problema de quantidade maior ou a menor
                cmd.BindByName = true;

                cmd.Parameters.Add(new OracleParameter("Nome", nome.ToUpperIgnoreNull()));
                cmd.Parameters.Add(new OracleParameter("Descricao", descricao.ToUpperIgnoreNull()));
                cmd.Parameters.Add(new OracleParameter("Itens", itens));
                cmd.Parameters.Add(new OracleParameter("Page", page));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var curso = new Curso(Convert.ToString(reader["nome"]), Convert.ToString(reader["descricao"]), Guid.Parse(Convert.ToString(reader["id"])));

                        _cursos.Add(curso);

                    }
                }
            }

            //return _cursos.Skip((page - 1) * itens).Take(itens);
            return _cursos;

        }


    }
}
