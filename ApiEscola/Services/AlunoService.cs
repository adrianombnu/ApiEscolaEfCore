using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Repository;
using System;

namespace ApiEscola.Services
{
    public class AlunoService
    {
        private readonly AlunoRepository _alunoRepository;
        private readonly MateriaRepository _materiaRepository;
        private readonly TurmaRepository _turmaRepository;

        public AlunoService(AlunoRepository alunoRepository, MateriaRepository materiaRepository, TurmaRepository turmaRepository)
        {
            _alunoRepository = alunoRepository;
            _materiaRepository = materiaRepository;
            _turmaRepository = turmaRepository;
        }

        public ResultadoDTO Cadastrar(Guid idTurma, Aluno aluno)
        {
            if (_alunoRepository.VerificaSeAlunoJaCadastrado(aluno.Documento, idTurma))
                return ResultadoDTO.ErroResultado("Aluno já está matriculado na turma informada!");

            var turma = _turmaRepository.BuscaTurmaPeloId(idTurma);

            if (turma is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada.");

            if (aluno.DataNascimento.Date >= DateTime.Now.Date)
                return ResultadoDTO.ErroResultado("Data de nascimento inválida.");

            if (aluno.IdMaterias.Count > 3)
                return ResultadoDTO.ErroResultado("Aluno somente pode estar matriculado em até 3 matérias por curso.");

            foreach (var id in aluno.IdMaterias)
            {
                var materia = _materiaRepository.BuscaMateriaPeloId(id);

                if (materia is null)
                    return ResultadoDTO.ErroResultado("Materia informada não existe!");

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
    }
}
