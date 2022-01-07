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

        

        

        
        


    }
}
