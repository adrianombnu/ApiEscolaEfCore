using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Repository;
using System;
using System.Collections.Generic;

namespace ApiEscola.Services
{
    public class ProfessorService
    {
        private readonly ProfessorRepository _professorRepository;

        public ProfessorService(ProfessorRepository repository)
        {
            _professorRepository = repository;
        }

        public ResultadoDTO Cadastrar(Professor professor)
        {
            if (_professorRepository.BuscaProfessorPeloDocumento(professor.Documento))
                return ResultadoDTO.ErroResultado("Já existe um professor cadastrado com o número de documento informado!");

            if (!_professorRepository.Cadastrar(professor))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar o professor!");

            return ResultadoDTO.SucessoResultado(professor);

        }

        public ResultadoDTO AtualizarProfessor(Professor professor)
        {
            var professorAtual = _professorRepository.BuscaProfessorPeloId(professor.Id);

            if (professorAtual is null)
                return ResultadoDTO.ErroResultado("Professor não encontrado!");

            if (_professorRepository.VerificaSeProfessorJaCadastrado(professor.Documento, professor.Id))
                return ResultadoDTO.ErroResultado("Já existe um professor cadastrado com o documento informado!");

            if (!_professorRepository.AtualizarProfessor(professor))
                return ResultadoDTO.ErroResultado("Professor não pode ser atualizado!");

            return ResultadoDTO.SucessoResultado(professor);

        }

        public ResultadoDTO RemoverProfessor(Guid id)
        {
            var professor = _professorRepository.BuscaProfessorPeloId(id);

            if (professor is null)
                return ResultadoDTO.ErroResultado("Professor não encontrado");

            if (_professorRepository.VerificaSePossuiMateriaVinculada(id))
                return ResultadoDTO.ErroResultado("Este professor já possui materia(s) vinculada(s), favor remove-la(s)");

            if (_professorRepository.RomoverProfessor(id))
                return ResultadoDTO.SucessoResultado();
            else
                return ResultadoDTO.ErroResultado("Erro ao remover o professor");

        }
        public ResultadoDTO ListarProfessores(string? nome = null, string? sobrenome = null, DateTime? dataDeNascimento = null, string? documento = null , int page = 1, int itens = 50)
        {
            var listaProfessores = _professorRepository.ListarProfessores(nome, sobrenome, dataDeNascimento, documento, page, itens);

            return ResultadoDTO.SucessoResultado(listaProfessores);

        }
    }
}
