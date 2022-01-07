using ApiEscolaEfCore.DTOs;
using ApiEscolaEfCore.Entities;
using ApiEscolaEfCore.Repository;
using Dominio;
using Dominio.Entities;
using System;
using System.Collections.Generic;

#nullable enable
namespace ApiEscolaEfCore.Services
{
    public class ProfessorService
    {
        private readonly ProfessorRepository _professorRepository;
        private readonly IProfessorRepository _iProfessorRepository;

        public ProfessorService(ProfessorRepository repository, IProfessorRepository iProfessorRepository)
        {
            _professorRepository = repository;
            _iProfessorRepository = iProfessorRepository;
        }

        public ResultadoDTO Cadastrar(Professor professor)
        {
            if (_iProfessorRepository.BuscaProfessorPeloDocumento(professor.Documento))
                return ResultadoDTO.ErroResultado("Já existe um professor cadastrado com o número de documento informado!");

            if (!_professorRepository.Cadastrar(professor))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar o professor!");

            return ResultadoDTO.SucessoResultado(professor);

        }

        public ResultadoDTO AtualizarProfessor(Professor professor)
        {
            var professorAtual = _iProfessorRepository.BuscarPeloId(professor.Id);

            if (professorAtual is null)
                return ResultadoDTO.ErroResultado("Professor não encontrado!");

            if (_iProfessorRepository.VerificaSeProfessorJaCadastrado(professor.Documento, professor.Id))
                return ResultadoDTO.ErroResultado("Já existe um professor cadastrado com o documento informado!");

            if (!_professorRepository.AtualizarProfessor(professor))
                return ResultadoDTO.ErroResultado("Professor não pode ser atualizado!");

            return ResultadoDTO.SucessoResultado(professor);

        }

        public ResultadoDTO RemoverProfessor(Guid id)
        {
            var professor = _iProfessorRepository.BuscarPeloId(id);

            if (professor is null)
                return ResultadoDTO.ErroResultado("Professor não encontrado");

            if (_iProfessorRepository.VerificaSePossuiMateriaVinculada(id))
                return ResultadoDTO.ErroResultado("Este professor já possui materia(s) vinculada(s), favor remove-la(s)");

            if (_professorRepository.RomoverProfessor(id))
                return ResultadoDTO.SucessoResultado();
            else
                return ResultadoDTO.ErroResultado("Erro ao remover o professor");

        }

        public ResultadoDTO BuscarPeloId(Guid id)
        {
            var professor = _iProfessorRepository.BuscarPeloId(id);

            if (professor is null)
                return ResultadoDTO.ErroResultado("Professor não encontrado");

            return ResultadoDTO.SucessoResultado(professor);

        }

        public ResultadoDTO ListarProfessores(string? nome = null, string? sobrenome = null, DateTime? dataDeNascimento = null, string? documento = null , int page = 1, int itens = 50)
        {
            var listaProfessores = _iProfessorRepository.ListarProfessores(nome, sobrenome, dataDeNascimento, documento, page, itens);

            return ResultadoDTO.SucessoResultado(listaProfessores);

        }
    }
}
