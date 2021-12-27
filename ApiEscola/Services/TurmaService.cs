using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Repository;
using System.Collections.Generic;

namespace ApiEscola.Services
{
    public class TurmaService
    {
        private readonly TurmaRepository _turmaRepository;

        public TurmaService(TurmaRepository repository)
        {
            _turmaRepository = repository;
        }

        /*
        public ResultadoDTO Cadastrar(Turma turma)
        {
            if (_cursoRepository.BuscaCursoPeloNome(curso.Nome))
                return ResultadoDTO.ErroResultado("Já existe um curso cadastrado com o nome informado!");

            if (!_cursoRepository.Cadastrar(curso))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar o curso!");

            return ResultadoDTO.SucessoResultado(curso);

        }
        */


        public IEnumerable<Turma> ListarTurmas()
        {
            return _turmaRepository.ListarTurmas();

        }
    }
}
