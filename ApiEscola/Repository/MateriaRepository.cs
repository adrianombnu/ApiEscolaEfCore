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
