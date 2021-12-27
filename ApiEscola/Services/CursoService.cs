using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Repository;
using System.Collections.Generic;

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
            var cursoAtual = _cursoRepository.BuscaCursoPeloId(curso.Id);

            if (cursoAtual is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado!");

            if(_cursoRepository.VerificaSeCursoJaCadastrado(curso.Nome, curso.Descricao, curso.Id))
                return ResultadoDTO.ErroResultado("Já existe um curso cadastrado com os dados informados!");

            if (!_cursoRepository.AtualizarCurso(curso))
                return ResultadoDTO.ErroResultado("Curso não pode ser atualizado!");

            return ResultadoDTO.SucessoResultado(curso);

        }

        public IEnumerable<Curso> ListarCursos()
        {
            return _cursoRepository.ListarCursos();

        }



    }
}
