using Dominio.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace ApiEscolaEfCore.Repository
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

        


    }
}
