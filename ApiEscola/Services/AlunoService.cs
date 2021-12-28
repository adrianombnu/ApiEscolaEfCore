using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ApiEscola.Services
{
    public class AlunoService
    {
        private readonly AlunoRepository _alunoRepository;
        private readonly MateriaRepository _materiaRepository;
        private readonly TurmaRepository _turmaRepository;
        private readonly IConfiguration _configuration;

        public AlunoService(AlunoRepository alunoRepository, MateriaRepository materiaRepository, TurmaRepository turmaRepository, IConfiguration configuration)
        {
            _alunoRepository = alunoRepository;
            _materiaRepository = materiaRepository;
            _turmaRepository = turmaRepository;
            _configuration = configuration;
        }

        public ResultadoDTO Cadastrar(Guid idTurma, Aluno aluno)
        {
            var LimiteMaximoDeMateriasPorAlunoPorCurso = _configuration.GetValue<int>("LimiteMaximoDeMateriasPorAlunoPorCurso");

            if (_alunoRepository.VerificaSeAlunoJaCadastrado(aluno.Documento, idTurma))
                return ResultadoDTO.ErroResultado("Aluno já está matriculado na turma informada!");

            var turma = _turmaRepository.BuscaTurmaPeloId(idTurma);

            if (turma is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada.");

            if (aluno.DataNascimento.Date >= DateTime.Now.Date)
                return ResultadoDTO.ErroResultado("Data de nascimento inválida.");

            if (aluno.IdMaterias.Count > LimiteMaximoDeMateriasPorAlunoPorCurso)
                return ResultadoDTO.ErroResultado("Aluno somente pode estar matriculado em até " + LimiteMaximoDeMateriasPorAlunoPorCurso + " matérias por curso.");

            foreach (var idMateria in aluno.IdMaterias)
            {
                if(!_turmaRepository.VerificaVinculoMateriaComATurma(idMateria, idTurma))
                    return ResultadoDTO.ErroResultado("Materia não cadastrada para o curso informado!");

            }

            if (!_alunoRepository.Cadastrar(idTurma, aluno))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar o aluno!");

            return ResultadoDTO.SucessoResultado(aluno);

        }

        public ResultadoDTO RemoverAluno(Guid id)
        {
            var aluno = _alunoRepository.BuscarAlunoPeloId(id);

            if (aluno is null)
                return ResultadoDTO.ErroResultado("Aluno não encontrado.");

            if (_alunoRepository.RomoverAluno(aluno))
                return ResultadoDTO.SucessoResultado();
            else
                return ResultadoDTO.ErroResultado("Erro ao remover o aluno.");

        }

        public ResultadoDTO BuscarAlunoPeloId(Guid id)
        {
            var aluno = _alunoRepository.BuscarAlunoPeloId(id);

            if (aluno is null)
                return ResultadoDTO.ErroResultado("Aluno não encontrado");

            return ResultadoDTO.SucessoResultado(aluno);

        }

        public IEnumerable<Aluno> ListarAlunos(string? nome = null, string? sobrenome = null, DateTime? dataDeNascimento = null, int page = 1, int itens = 50)
        {
            return _alunoRepository.ListarAlunos(nome, sobrenome,dataDeNascimento, page, itens);

        }
    }
}
