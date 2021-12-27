using ApiEscola.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

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

                using var cmd = new OracleCommand(
                    @"INSERT INTO APPACADEMY.CURSO
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

                using var cmd = new OracleCommand(
                    @"UPDATE APPACADEMY.CURSO
                        SET id = :id,
                            nome = :Nome, 
                            descricao = :Descricao
                      where id = :id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", curso.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", curso.Nome));
                cmd.Parameters.Add(new OracleParameter("Descricao", curso.Descricao));

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

                using var cmd = new OracleCommand(@"select * from curso where nome = :nome", conn);

                cmd.Parameters.Add(new OracleParameter("nome", nomeCurso));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    cursoEncontrado = true;

            }

            return cursoEncontrado;

        }

        public bool VerificaSeCursoJaCadastrado(string nomeCurso, string descricao, Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var cursoEncontrado = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"select * from curso where nome = :nome and descricao = :descricao and id <> :id", conn);

                cmd.Parameters.Add(new OracleParameter("nome", nomeCurso));
                cmd.Parameters.Add(new OracleParameter("descricao", descricao));
                cmd.Parameters.Add(new OracleParameter("id", id.ToString()));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    cursoEncontrado = true;

            }

            return cursoEncontrado;

        }

        public Curso BuscaCursoPeloId(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"select * from curso where id = :id", conn);

                cmd.Parameters.Add(new OracleParameter("id", id.ToString()));

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

        public IEnumerable<Curso> ListarCursos()
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"select * from curso", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var curso = new Curso(Convert.ToString(reader["nome"]), Convert.ToString(reader["descricao"]), Guid.Parse(Convert.ToString(reader["id"])));

                        _cursos.Add(curso);

                    }
                }
            }

            return _cursos;

        }


    }
}
