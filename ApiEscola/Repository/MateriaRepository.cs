using ApiEscola.Entities;
using ApiEscola.Extensions;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiEscola.Repository
{

    public class MateriaRepository
    {
        private readonly IConfiguration _configuration;
        private readonly List<Materia> _materias;

        public MateriaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _materias ??= new List<Materia>();

        }

        public bool Cadastrar(Materia materia)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(
                    @"INSERT INTO APPACADEMY.materia
                        (ID, NOME, IDPROFESSOR)
                      VALUES(:Id,:Nome,:IdProfessor)", conn);

                cmd.Parameters.Add(new OracleParameter("Id", materia.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", materia.Nome));
                cmd.Parameters.Add(new OracleParameter("IdProfessor", materia.IdProfessor.ToString()));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool AtualizarMateria(Materia materia)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(
                    @"UPDATE APPACADEMY.materia
                        SET ID = :Id,
                            NOME = :Nome, 
                            IDPROFESSOR = :IdProfessor
                      WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", materia.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", materia.Nome));
                cmd.Parameters.Add(new OracleParameter("IdProfessor", materia.IdProfessor.ToString()));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool RomoverMateria(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"DELETE FROM APPACADEMY.materia                        
                                                          WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", id.ToString()));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool VerificaSePossuiTurmaVinculada(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM turma_materia tm 
                                                            WHERE tm.idmateria = :IdMateria", conn);

                cmd.Parameters.Add(new OracleParameter("IdMateria", id.ToString()));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    return true;

            }

            return false;

        }

        public Materia BuscaMateriaPeloId(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM MATERIA WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", id.ToString()));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Materia(Convert.ToString(reader["nome"]), Guid.Parse(Convert.ToString(reader["idProfessor"])), Guid.Parse(Convert.ToString(reader["id"])));

                    }
                }

            }

            return null;

        }

        public bool VerificaSeMateriaJaCadastrada(string nomeMateria, Guid idMateria, Guid idProfessor, bool consideraIdDifente = false)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                var query = (@"SELECT * FROM MATERIA M WHERE 1 = 1");

                var sb = new StringBuilder(query);

                sb.Append(" AND UPPER(M.NOME) = :NomeMateria ");
                sb.Append(" AND M.IDPROFESSOR = :IdProfessor ");

                if (consideraIdDifente)
                    sb.Append(" AND M.ID <> :IdMateria ");

                using var cmd = new OracleCommand(sb.ToString(), conn);

                //Esse bind serve para que quando, for passado mais parametros do que o necessário para montar o comando sql, devido a ser criado de forma dinamica, vamos evitar que dê
                //problema de quantidade maior ou a menor
                cmd.BindByName = true;

                cmd.Parameters.Add(new OracleParameter("IdMateria", idMateria.ToString()));
                cmd.Parameters.Add(new OracleParameter("NomeMateria", nomeMateria.ToUpperIgnoreNull()));
                cmd.Parameters.Add(new OracleParameter("IdProfessor", idProfessor.ToString()));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    return true;

            }

            return false;

        }

        public IEnumerable<Materia> ListarMaterias(string? nome = null, int page = 1, int itens = 50)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                /*
                using var cmd = new OracleCommand();

                var query = (@"SELECT * FROM (SELECT ROWNUM AS RN, M.* FROM MATERIA M WHERE 1 = 1");

                var sb = new StringBuilder(query);

                if (!string.IsNullOrEmpty(nome))
                {
                    sb.Append(" AND UPPER(M.NOME) LIKE '%' || :Nome || '%'");
                    cmd.Parameters.Add(new OracleParameter("Nome", nome.ToUpperIgnoreNull()));
                }

                sb.Append(" ORDER BY ROWNUM) MATERIAS");
                sb.Append(" WHERE ROWNUM <= :Itens AND MATERIAS.RN > (:Page -1) * :Itens");
                cmd.Parameters.Add(new OracleParameter("Itens", itens));
                cmd.Parameters.Add(new OracleParameter("Page", page));

                cmd.Connection = conn;
                cmd.CommandText = sb.ToString();
                */

                var query = (@"SELECT * FROM (SELECT ROWNUM AS RN, M.* FROM MATERIA M WHERE 1 = 1");

                var sb = new StringBuilder(query);

                if (!string.IsNullOrEmpty(nome))
                    sb.Append(" AND UPPER(M.NOME) LIKE '%' || :Nome || '%'");

                sb.Append(" ORDER BY ROWNUM) MATERIAS");
                sb.Append(" WHERE ROWNUM <= :Itens AND MATERIAS.RN > (:Page -1) * :Itens");

                using var cmd = new OracleCommand(sb.ToString(), conn);

                //Esse bind serve para que quando, for passado mais parametros do que o necessário para montar o comando sql, devido a ser criado de forma dinamica, vamos evitar que dê
                //problema de quantidade maior ou a menor
                cmd.BindByName = true;

                cmd.Parameters.Add(new OracleParameter("Nome", nome.ToUpperIgnoreNull()));
                cmd.Parameters.Add(new OracleParameter("Itens", itens));
                cmd.Parameters.Add(new OracleParameter("Page", page));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var materia = new Materia(Convert.ToString(reader["nome"]), Guid.Parse(Convert.ToString(reader["idProfessor"])), Guid.Parse(Convert.ToString(reader["id"])));

                        _materias.Add(materia);

                    }
                }
            }

            //return _materias.Skip((page - 1) * itens).Take(itens);
            return _materias;

        }


    }
}
