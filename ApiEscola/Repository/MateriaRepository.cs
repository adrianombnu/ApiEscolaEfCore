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

        public bool AtualizarMateria(Materia materia)
        {
            var conexao = _configuration.GetSection("ConnectionStrings").GetValue<string>("Conexao");
            var retorno = false;

            using (var conn = new OracleConnection(conexao))
            {
                conn.Open();

                using var cmd = new OracleCommand(
                    @"UPDATE APPACADEMY.materia
                        SET id = :id,
                            nome = :Nome, 
                            idProfessor = :IdProfessor
                      where id = :id", conn);

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

                using var cmd = new OracleCommand(
                    @"DELETE FROM APPACADEMY.materia                        
                      WHERE id = :Id", conn);

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

                using var cmd = new OracleCommand(@"SELECT * FROM turma_materia tm WHERE tm.idmateria  = :idMateria", conn);

                cmd.Parameters.Add(new OracleParameter("idMateria", id.ToString()));

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

                using var cmd = new OracleCommand(@"select * from materia where id = :id", conn);

                cmd.Parameters.Add(new OracleParameter("id", id.ToString()));

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
                var comandoString = string.Empty;

                if (consideraIdDifente)
                    comandoString = @"select * from materia 
                                        where id <> :IdMateria 
                                            and nome = :NomeMateria 
                                            and idProfessor = :IdProfessor";

                else
                    comandoString = @"select * from materia 
                                        where nome = :NomeMateria 
                                          and idProfessor = :IdProfessor";

                using var cmd = new OracleCommand(comandoString, conn);

                if (consideraIdDifente)
                {
                    cmd.Parameters.Add(new OracleParameter("IdMateria", idMateria.ToString()));
                    cmd.Parameters.Add(new OracleParameter("NomeMateria", nomeMateria));
                    cmd.Parameters.Add(new OracleParameter("IdProfessor", idProfessor.ToString()));

                }
                else
                {
                    cmd.Parameters.Add(new OracleParameter("NomeMateria", nomeMateria));
                    cmd.Parameters.Add(new OracleParameter("IdProfessor", idProfessor.ToString()));
                }

                using var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                    return true;

            }

            return false;

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
