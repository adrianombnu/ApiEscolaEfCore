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

    public class ProfessorRepository
    {
        private readonly IConfiguration _configuration;
        private readonly List<Professor> _professores;

        public ProfessorRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _professores ??= new List<Professor>();

        }

        public bool Cadastrar(Professor professor)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"INSERT INTO APPACADEMY.PROFESSOR
                                                    (ID, NOME, SOBRENOME, DATADENASCIMENTO, DOCUMENTO)
                                                    VALUES(:Id,:Nome,:Sobrenome, :DataDeNascimento,:Documento)", conn);

                cmd.Parameters.Add(new OracleParameter("Id", professor.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", professor.Nome));
                cmd.Parameters.Add(new OracleParameter("Sobrenome", professor.Sobrenome));
                cmd.Parameters.Add(new OracleParameter("DataDeNascimento", professor.DataNascimento));
                cmd.Parameters.Add(new OracleParameter("Documento", professor.Documento));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool AtualizarProfessor(Professor professor)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"UPDATE APPACADEMY.professor
                                                       SET ID = :Id,
                                                           NOME = :Nome, 
                                                           SOBRENOME = :Sobrenome,
                                                           DATADENASCIMENTO =: DataDeNascimento,
                                                           DOCUMENTO = :Documento
                                                     WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", professor.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", professor.Nome));
                cmd.Parameters.Add(new OracleParameter("Sobrenome", professor.Sobrenome));
                cmd.Parameters.Add(new OracleParameter("DataDeNascimento", professor.DataNascimento));
                cmd.Parameters.Add(new OracleParameter("Documento", professor.Documento));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool RomoverProfessor(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"DELETE FROM APPACADEMY.professor                        
                                                          WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", id.ToString()));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool BuscaProfessorPeloDocumento(string documento)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var cursoEncontrado = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();


                using var cmd = new OracleCommand(@"SELECT * FROM PROFESSOR 
                                                            WHERE DOCUMENTO = :Documento", conn);

                cmd.Parameters.Add(new OracleParameter("Documento", documento));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    cursoEncontrado = true;

            }

            return cursoEncontrado;

        }

        public Professor BuscaProfessorPeloId(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM PROFESSOR WHERE ID = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", id.ToString()));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Professor(Convert.ToString(reader["nome"]), Convert.ToString(reader["sobrenome"]), Convert.ToDateTime(reader["dataDeNascimento"]), Convert.ToString(reader["documento"]), Guid.Parse(Convert.ToString(reader["id"])));


                    }
                }

            }

            return null;

        }

        public bool VerificaSePossuiMateriaVinculada(Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM PROFESSOR P
                                                       INNER JOIN MATERIA M 
                                                               ON P.id = M.idProfessor 
                                                            WHERE P.ID = :IdProfessor", conn);

                cmd.Parameters.Add(new OracleParameter("IdProfessor", id.ToString()));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    return true;

            }

            return false;

        }

        public bool VerificaSeProfessorJaCadastrado(string documento, Guid id)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"SELECT * FROM PROFESSOR 
                                                            WHERE ID <> :Id 
                                                              AND DOCUMENTO = :Documento " , conn);

                cmd.Parameters.Add(new OracleParameter("Id", id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Documento", documento));

                using var reader = cmd.ExecuteReader();

                if(reader.HasRows)
                    return true;

            }

            return false;

        }

        public IEnumerable<Professor> ListarProfessores(string? nome = null, string? sobrenome = null, DateTime? dataDeNascimento = null, string? documento = null, int page = 1, int itens = 50)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                /*
                using var cmd = new OracleCommand();

                var query = (@"SELECT * FROM (SELECT ROWNUM AS RN, P.* FROM PROFESSOR P WHERE 1 = 1");

                var sb = new StringBuilder(query);

                if (!string.IsNullOrEmpty(nome))
                {
                    sb.Append(" AND UPPER(P.NOME) LIKE '%' || :Nome || '%'");
                    cmd.Parameters.Add(new OracleParameter("Nome", nome.ToUpperIgnoreNull()));
                }

                if (!string.IsNullOrEmpty(sobrenome))
                {
                    sb.Append(" AND UPPER(P.SOBRENOME) LIKE '%' || :Sobrenome || '%'");
                    cmd.Parameters.Add(new OracleParameter("Sobrenome", sobrenome.ToUpperIgnoreNull()));
                }

                if (!string.IsNullOrEmpty(dataDeNascimento.ToString()))
                {
                    sb.Append(" AND to_char(P.DATADENASCIMENTO,'dd/mm/rrrr') = :DataDeNascimento");
                    cmd.Parameters.Add(new OracleParameter("DataDeNascimento", dataDeNascimento.Value.ToString("dd/MM/yyyy")));
                }

                if (!string.IsNullOrEmpty(documento))
                {
                    sb.Append(" AND P.DOCUMENTO = :Documento");
                    cmd.Parameters.Add(new OracleParameter("Documento", documento));
                }

                sb.Append(" ORDER BY ROWNUM) PROFESSORES");
                sb.Append(" WHERE ROWNUM <= :Itens AND PROFESSORES.RN > (:Page -1) * :Itens");
                cmd.Parameters.Add(new OracleParameter("Itens", itens));
                cmd.Parameters.Add(new OracleParameter("Page", page));

                cmd.Connection = conn;
                cmd.CommandText = sb.ToString();
                */

                var query = (@"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ROWID) AS RN,
                                                     P.* 
                                                FROM PROFESSOR P 
                                               WHERE 1 = 1");

                var sb = new StringBuilder(query);

                if (!string.IsNullOrEmpty(nome))
                    sb.Append(" AND UPPER(P.NOME) LIKE '%' || :Nome || '%' ");

                if (!string.IsNullOrEmpty(sobrenome))
                    sb.Append(" AND UPPER(P.SOBRENOME) LIKE '%' || :Sobrenome || '%' ");

                if (!string.IsNullOrEmpty(dataDeNascimento.ToString()))
                    sb.Append(" AND to_char(P.DATADENASCIMENTO,'dd/mm/rrrr') = :DataDeNascimento ");

                if (!string.IsNullOrEmpty(documento))
                    sb.Append(" AND P.DOCUMENTO = :Documento");

                sb.Append(" ) ALUNOS");
                sb.Append(" WHERE ROWNUM <= :Itens AND ALUNOS.RN > (:Page -1) * :Itens");

                using var cmd = new OracleCommand(sb.ToString(), conn);

                //Esse bind serve para que quando, for passado mais parametros do que o necessário para montar o comando sql, devido a ser criado de forma dinamica, vamos evitar que dê
                //problema de quantidade maior ou a menor
                cmd.BindByName = true;

                cmd.Parameters.Add(new OracleParameter("Nome", nome.ToUpperIgnoreNull()));
                cmd.Parameters.Add(new OracleParameter("Sobrenome", sobrenome.ToUpperIgnoreNull()));
                cmd.Parameters.Add(new OracleParameter("DataDeNascimento", (dataDeNascimento.HasValue ? dataDeNascimento.Value.ToString("dd/MM/yyyy") : "null")));
                cmd.Parameters.Add(new OracleParameter("Documento", documento));
                cmd.Parameters.Add(new OracleParameter("Itens", itens));
                cmd.Parameters.Add(new OracleParameter("Page", page));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var professor = new Professor(Convert.ToString(reader["nome"]), Convert.ToString(reader["sobrenome"]), Convert.ToDateTime(reader["dataDeNascimento"]), Convert.ToString(reader["documento"]), Guid.Parse(Convert.ToString(reader["id"])));

                        _professores.Add(professor);

                    }
                }
            }

            //return _professores.Skip((page - 1) * itens).Take(itens);
            return _professores;

        }


    }
}
