using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ApiEscola.Services
{
    public class MateriaService
    {
        private readonly MateriaRepository _materiaRepository;
        private readonly ProfessorRepository _professorRepository;

        public MateriaService(MateriaRepository materiaRepository, ProfessorRepository professorRepository)
        {
            _materiaRepository = materiaRepository;
            _professorRepository = professorRepository;
        }

        public ResultadoDTO Cadastrar(Materia materia)
        {
            if (_materiaRepository.BuscaMateriaPeloNome(materia.Nome))
                return ResultadoDTO.ErroResultado("Já existe uma materia cadastrada com o nome informado!");

            var professor = _professorRepository.BuscaProfessorPeloId(materia.IdProfessor);

            if (professor is null)
                return ResultadoDTO.ErroResultado("O prefessor informado não foi encontrado!");

            if (!_materiaRepository.Cadastrar(materia))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar a materia!");

            return ResultadoDTO.SucessoResultado(materia);

        }

        public ResultadoDTO BuscaMateriaPeloId(Guid id)
        {
            var materia = _materiaRepository.BuscaMateriaPeloId(id);

            if (materia is null)
                return ResultadoDTO.ErroResultado("Materia não encontrada");

            return null;

        }

        public IEnumerable<Materia> ListarMaterias()
        {
            return _materiaRepository.ListarMaterias();

        }
    }
}
