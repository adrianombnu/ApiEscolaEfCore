using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Repository;
using System;
using System.Collections.Generic;

namespace ApiEscola.Services
{
    public class TurmaService
    {
        private readonly TurmaRepository _turmaRepository;
        private readonly CursoRepository _cursoRepository;
        private readonly MateriaRepository _materiaRepository;

        public TurmaService(TurmaRepository turmaRepository, CursoRepository cursoRepository, MateriaRepository materiaRepository)
        {
            _turmaRepository = turmaRepository;
            _cursoRepository = cursoRepository;
            _materiaRepository = materiaRepository;
        }

        
        public ResultadoDTO Cadastrar(Turma turma)
        {
            if (_turmaRepository.BuscaTurmaPeloNome(turma.Nome))
                return ResultadoDTO.ErroResultado("Já existe uma turma cadastrada com o nome informado!");

            var curso = _cursoRepository.BuscaCursoPeloId(turma.IdCurso);

            if (curso is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado");

            if(turma.DataInicio.Date < DateTime.Now.Date)
                return ResultadoDTO.ErroResultado("Data de inicio não pode ser menor que " + DateTime.Now.Date.ToString("dd/MM/yyyy"));

            if (turma.DataFim.Date < turma.DataInicio.Date)
                return ResultadoDTO.ErroResultado("Data fim não pode ser menor que " + turma.DataInicio.Date.ToString("dd/MM/yyyy"));

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
        
        /*
        public IEnumerable<Turma> ListarTurmas()
        {
            return _turmaRepository.ListarTurmas();

        }*/

    }
}
