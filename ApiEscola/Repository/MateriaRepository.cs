using Dominio.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

#nullable enable
namespace ApiEscolaEfCore.Repository
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

        

    }
}
