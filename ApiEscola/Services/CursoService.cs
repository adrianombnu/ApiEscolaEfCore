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
    public class CursoService
    {
        private readonly CursoRepository _cursoRepository;
        private readonly ICursoRepository cursoRepository;
        private readonly ICursoRepository _iCursoRepository;

        public CursoService(CursoRepository repository, ICursoRepository iCursoRepository)
        {
            _cursoRepository = repository;
            _iCursoRepository = iCursoRepository;
        }
                
        public ResultadoDTO Cadastrar(Curso curso)
        {
            if (_iCursoRepository.BuscaCursoPeloNome(curso.Nome))
                return ResultadoDTO.ErroResultado("Já existe um curso cadastrado com o nome informado!");

            if (!_cursoRepository.Cadastrar(curso))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar o curso!");

            return ResultadoDTO.SucessoResultado(curso);

        }

        public ResultadoDTO AtualizarCurso(Curso curso)
        {
            var cursoAtual = _iCursoRepository.BuscarPeloId(curso.Id);

            if (cursoAtual is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado!");

            if(_iCursoRepository.VerificaSeCursoJaCadastrado(curso.Nome, curso.Id))
                return ResultadoDTO.ErroResultado("Já existe um curso cadastrado com os dados informados!");

            if (!_cursoRepository.AtualizarCurso(curso))
                return ResultadoDTO.ErroResultado("Curso não pode ser atualizado!");

            return ResultadoDTO.SucessoResultado(curso);

        }

        public ResultadoDTO RemoverCurso(Guid id)
        {
            var curso = _iCursoRepository.BuscarPeloId(id);

            if (curso is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado");

            if(_iCursoRepository.VerificaSeCursoPossuiTurma(id))
                return ResultadoDTO.ErroResultado("Ester curso já possui uma turma cadastrada, favor remove-la");

            if (_cursoRepository.RomoveCurso(id))
                return ResultadoDTO.SucessoResultado();
            else
                return ResultadoDTO.ErroResultado("Erro ao remover o curso");

        }

        public ResultadoDTO BuscarCursoPeloId(Guid id)
        {
            var curso = _iCursoRepository.BuscarPeloId(id);

            if (curso is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado.");

            return ResultadoDTO.SucessoResultado(curso);

        }

        public ResultadoDTO ListarCursos(string? nome = null, string? descricao = null, int page = 1, int itens = 50)
        {
            var listaCursos = _iCursoRepository.ListarCursos(nome, descricao, page, itens);

            return ResultadoDTO.SucessoResultado(listaCursos);

        }



    }
}
