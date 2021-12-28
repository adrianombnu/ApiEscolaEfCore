using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ApiEscola.Services
{
    public class TurmaService
    {
        private readonly TurmaRepository _turmaRepository;
        private readonly CursoRepository _cursoRepository;
        private readonly MateriaRepository _materiaRepository;
        private readonly IConfiguration _configuration;

        public TurmaService(TurmaRepository turmaRepository, CursoRepository cursoRepository, MateriaRepository materiaRepository, IConfiguration configuration)
        {
            _turmaRepository = turmaRepository;
            _cursoRepository = cursoRepository;
            _materiaRepository = materiaRepository;
            _configuration = configuration;
        }

        
        public ResultadoDTO Cadastrar(Turma turma)
        {
            var LimiteMinimoDeMateriasPorTurma = _configuration.GetValue<int>("LimiteMinimoDeMateriasPorTurma");
            var LimiteMaximoDeMateriasPorTurma = _configuration.GetValue<int>("LimiteMaximoDeMateriasPorTurma");

            if (_turmaRepository.BuscaTurmaPeloNome(turma.Nome))
                return ResultadoDTO.ErroResultado("Já existe uma turma cadastrada com o nome informado!");

            var curso = _cursoRepository.BuscaCursoPeloId(turma.IdCurso);

            if (curso is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado");

            if(turma.DataInicio.Date < DateTime.Now.Date)
                return ResultadoDTO.ErroResultado("Data de inicio não pode ser menor que " + DateTime.Now.Date.ToString("dd/MM/yyyy"));

            if (turma.DataFim.Date < turma.DataInicio.Date)
                return ResultadoDTO.ErroResultado("Data fim não pode ser menor que " + turma.DataInicio.Date.ToString("dd/MM/yyyy"));

            if(turma.IdMaterias.Count < LimiteMinimoDeMateriasPorTurma)
                return ResultadoDTO.ErroResultado("Turma deve conter no minímo " + LimiteMinimoDeMateriasPorTurma + " matéria em sua grade curricular.");

            if (turma.IdMaterias.Count > LimiteMaximoDeMateriasPorTurma)
                return ResultadoDTO.ErroResultado("Turma deve conter no máximo " + LimiteMaximoDeMateriasPorTurma  + " matérias em sua grade curricular.");

            foreach (var id in turma.IdMaterias)
            {
                var materia = _materiaRepository.BuscaMateriaPeloId(id);

                if(materia is null)
                    return ResultadoDTO.ErroResultado("Materia informada não existe!");

            }

            if (!_turmaRepository.Cadastrar(turma))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar a turma!");
            
            return ResultadoDTO.SucessoResultado(turma);
            
        }
        public ResultadoDTO BuscaTurmaPeloId(Guid id)
        {
            var turma = _turmaRepository.BuscaTurmaPeloId(id);

            if (turma is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada");

            return ResultadoDTO.SucessoResultado(turma);

        }

        public IEnumerable<Turma> ListarTurmas(string? nome = null, DateTime? dataInicio = null, DateTime? dataFim = null, int page = 1, int itens = 50)
        {
            return _turmaRepository.ListarTurmas(nome, dataInicio, dataFim, page, itens);

        }

    }
}
