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
            
            /*if (_materiaRepository.BuscaMateriaPeloNome(materia.Nome))
                return ResultadoDTO.ErroResultado("Já existe uma materia cadastrada com o nome informado!");
            */

            if (_materiaRepository.VerificaSeMateriaJaCadastrada(materia.Nome, materia.Id, materia.IdProfessor))
                return ResultadoDTO.ErroResultado("Matéria já cadastrada para o professor informado!");

            var professor = _professorRepository.BuscaProfessorPeloId(materia.IdProfessor);

            if (professor is null)
                return ResultadoDTO.ErroResultado("O prefessor informado não foi encontrado!");

            if (!_materiaRepository.Cadastrar(materia))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar a materia!");

            return ResultadoDTO.SucessoResultado(materia);

        }

        public ResultadoDTO AtualizarMateria(Materia materia)
        {
            var materiaAtual = _materiaRepository.BuscaMateriaPeloId(materia.Id);

            if (materiaAtual is null)
                return ResultadoDTO.ErroResultado("Matéria não encontrada!");

            if (_materiaRepository.VerificaSeMateriaJaCadastrada(materia.Nome, materia.Id, materia.IdProfessor, true))
                return ResultadoDTO.ErroResultado("Matéria já cadastrada para o professor informado!");

            var professor = _professorRepository.BuscaProfessorPeloId(materia.IdProfessor);

            if (professor is null)
                return ResultadoDTO.ErroResultado("O prefessor informado não foi encontrado!");

            if (!_materiaRepository.AtualizarMateria(materia))
                return ResultadoDTO.ErroResultado("Matéria não pode ser atualizada!");

            return ResultadoDTO.SucessoResultado(materia);

        }

        public ResultadoDTO RemoverMateria(Guid id)
        {
            var materia = _materiaRepository.BuscaMateriaPeloId(id);

            if (materia is null)
                return ResultadoDTO.ErroResultado("Matéria não encontrada");

            if (_materiaRepository.VerificaSePossuiTurmaVinculada(id))
                return ResultadoDTO.ErroResultado("Matéria já está vinculada a uma turma, favor remover o vinculo");

            if (_materiaRepository.RomoverMateria(id))
                return ResultadoDTO.SucessoResultado();
            else
                return ResultadoDTO.ErroResultado("Erro ao remover a matéria");

        }

        public ResultadoDTO BuscaMateriaPeloId(Guid id)
        {
            var materia = _materiaRepository.BuscaMateriaPeloId(id);

            if (materia is null)
                return ResultadoDTO.ErroResultado("Materia não encontrada");

            return ResultadoDTO.SucessoResultado(materia);

        }

        public IEnumerable<Materia> ListarMaterias()
        {
            return _materiaRepository.ListarMaterias();

        }
    }
}
