using ApiEscola.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

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

                using var cmd = new OracleCommand(
                    @"INSERT INTO APPACADEMY.PROFESSOR
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

                using var cmd = new OracleCommand(
                    @"UPDATE APPACADEMY.professor
                        SET id = :id,
                            nome = :Nome, 
                            sobrenome = :Sobrenome,
                            dataDeNascimento =: DataDeNascimento,
                            documento = :Documento
                      where id = :id", conn);

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

                using var cmd = new OracleCommand(
                    @"DELETE FROM APPACADEMY.professor                        
                      WHERE id = :Id", conn);

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


                using var cmd = new OracleCommand(@"select * from professor where documento = :documento", conn);

                cmd.Parameters.Add(new OracleParameter("documento", documento));

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

                using var cmd = new OracleCommand(@"select * from professor where id = :id", conn);

                cmd.Parameters.Add(new OracleParameter("id", id.ToString()));

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

                using var cmd = new OracleCommand(@"SELECT * FROM professor p INNER JOIN materia m ON p.id = M.idProfessor WHERE p.ID  = :idProfessor", conn);

                cmd.Parameters.Add(new OracleParameter("idProfessor", id.ToString()));

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

                using var cmd = new OracleCommand(@"select * from professor where id <> :id and documento = :documento " , conn);

                cmd.Parameters.Add(new OracleParameter("id", id.ToString()));
                cmd.Parameters.Add(new OracleParameter("documento", documento));

                using var reader = cmd.ExecuteReader();

                if(reader.HasRows)
                    return true;

            }

            return false;

        }

        public IEnumerable<Professor> ListarProfessores()
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"select * from professor", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var professor = new Professor(Convert.ToString(reader["nome"]), Convert.ToString(reader["sobrenome"]), Convert.ToDateTime(reader["dataDeNascimento"]), Convert.ToString(reader["documento"]), Guid.Parse(Convert.ToString(reader["id"])));

                        _professores.Add(professor);

                    }
                }
            }

            return _professores;

        }


    }
}
