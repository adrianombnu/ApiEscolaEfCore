using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Repository;
using System;
using System.Collections.Generic;

#nullable enable
namespace ApiEscola.Services
{
    public class CursoService
    {
        private readonly CursoRepository _cursoRepository;

        public CursoService(CursoRepository repository)
        {
            _cursoRepository = repository;
        }
                
        public ResultadoDTO Cadastrar(Curso curso)
        {
            if (_cursoRepository.BuscaCursoPeloNome(curso.Nome))
                return ResultadoDTO.ErroResultado("Já existe um curso cadastrado com o nome informado!");

            if (!_cursoRepository.Cadastrar(curso))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar o curso!");

            return ResultadoDTO.SucessoResultado(curso);

        }

        public ResultadoDTO AtualizarCurso(Curso curso)
        {
            var cursoAtual = _cursoRepository.BuscarCursoPeloId(curso.Id);

            if (cursoAtual is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado!");

            if(_cursoRepository.VerificaSeCursoJaCadastrado(curso.Nome, curso.Id))
                return ResultadoDTO.ErroResultado("Já existe um curso cadastrado com os dados informados!");

            if (!_cursoRepository.AtualizarCurso(curso))
                return ResultadoDTO.ErroResultado("Curso não pode ser atualizado!");

            return ResultadoDTO.SucessoResultado(curso);

        }

        public ResultadoDTO RemoverCurso(Guid id)
        {
            var curso = _cursoRepository.BuscarCursoPeloId(id);

            if (curso is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado");

            if(_cursoRepository.VerificaSeCursoPossuiTurma(id))
                return ResultadoDTO.ErroResultado("Ester curso já possui uma turma cadastrada, favor remove-la");

            if (_cursoRepository.RomoveCurso(id))
                return ResultadoDTO.SucessoResultado();
            else
                return ResultadoDTO.ErroResultado("Erro ao remover o curso");

        }

        public ResultadoDTO BuscarCursoPeloId(Guid id)
        {
            var curso = _cursoRepository.BuscarCursoPeloId(id);

            if (curso is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado.");

            return ResultadoDTO.SucessoResultado(curso);

        }

        public ResultadoDTO ListarCursos(string? nome = null, string? descricao = null, int page = 1, int itens = 50)
        {
            var listaCursos = _cursoRepository.ListarCursos(nome, descricao, page, itens);

            return ResultadoDTO.SucessoResultado(listaCursos);

        }



    }
}
