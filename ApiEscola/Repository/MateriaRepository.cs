using ApiEscola.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

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
                      VALUES(:Id,:Nome,:idProfessor)", conn);

                cmd.Parameters.Add(new OracleParameter("Id", materia.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", materia.Nome));
                cmd.Parameters.Add(new OracleParameter("idProfessor", materia.IdProfessor.ToString()));

                var rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    retorno = true;
            }

            return retorno;
        }

        public bool BuscaMateriaPeloNome(string nomeMateria)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var cursoEncontrado = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"select * from materia where nome = :nome", conn);

                cmd.Parameters.Add(new OracleParameter("nome", nomeMateria));

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    cursoEncontrado = true;

            }

            return cursoEncontrado;

        }

        public IEnumerable<Materia> ListarMaterias()
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(@"select * from materia", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var materia = new Materia(Convert.ToString(reader["nome"]), Guid.Parse(Convert.ToString(reader["idProfessor"])), Guid.Parse(Convert.ToString(reader["id"])));

                        _materias.Add(materia);

                    }
                }
            }

            return _materias;

        }


    }
}
