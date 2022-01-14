using ApiEscolaEfCore.DTOs;
using ApiEscolaEfCore.Entities;
using ApiEscolaEfCore.Repository;
using Dominio;
using Dominio.Entities;
using EfContext;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

#nullable enable
namespace ApiEscolaEfCore.Services
{
    public class AlunoService
    {
        private readonly AlunoRepository _alunoRepository;
        private readonly IAlunoRepository _iAlunoRepository;
        private readonly IAlunoRepositoryEfCore _iAlunoRepositoryEfCore;
        private readonly IMateriaRepository _iMateriaRepository;
        private readonly TurmaRepository _turmaRepository;
        private readonly ITurmaRepository _iTurmaRepository;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AlunoService(AlunoRepository alunoRepository,
                            IAlunoRepository iAlunoRepository,
                            IAlunoRepositoryEfCore iAlunoRepositoryEfCore,
                            IMateriaRepository iMateriaRepository,
                            TurmaRepository turmaRepository,
                            ITurmaRepository iTurmaRepository,
                            IConfiguration configuration,
                            IUnitOfWork unitOfWork)
        {
            _alunoRepository = alunoRepository;
            _iAlunoRepository = iAlunoRepository;
            _iMateriaRepository = iMateriaRepository;
            _turmaRepository = turmaRepository;
            _iTurmaRepository = iTurmaRepository;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _iAlunoRepositoryEfCore = iAlunoRepositoryEfCore;
        }

        public ResultadoDTO Cadastrar(Guid idTurma, Aluno aluno)
        {
            var LimiteMaximoDeMateriasPorAlunoPorCurso = _configuration.GetValue<int>("LimiteMaximoDeMateriasPorAlunoPorCurso");

            if (_iAlunoRepository.VerificaSeAlunoJaCadastrado(aluno.Documento, idTurma))
                return ResultadoDTO.ErroResultado("Aluno já está matriculado na turma informada!");

            var turma = _iTurmaRepository.BuscarPeloId(idTurma);

            if (turma is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada.");

            if (aluno.DataNascimento.Date >= DateTime.Now.Date)
                return ResultadoDTO.ErroResultado("Data de nascimento inválida.");

            if (aluno.IdMaterias.Count > LimiteMaximoDeMateriasPorAlunoPorCurso)
                return ResultadoDTO.ErroResultado("Aluno somente pode estar matriculado em até " + LimiteMaximoDeMateriasPorAlunoPorCurso + " matérias por curso.");

            foreach (var idMateria in aluno.IdMaterias)
            {
                if(!_iTurmaRepository.VerificaVinculoMateriaComATurma(idMateria, idTurma))
                    return ResultadoDTO.ErroResultado("Materia não cadastrada para o curso informado!");

            }

            /*if (!_alunoRepository.Cadastrar(idTurma, aluno))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar o aluno!");
            */

            _iAlunoRepositoryEfCore.Incluir(aluno);
            _unitOfWork.Commit();

            return ResultadoDTO.SucessoResultado(aluno);

        }

        public ResultadoDTO RemoverAluno(Guid id)
        {
            var aluno = _iAlunoRepository.BuscarPeloId(id);

            if (aluno is null)
                return ResultadoDTO.ErroResultado("Aluno não encontrado.");

            if (_alunoRepository.RomoverAluno(aluno))
                return ResultadoDTO.SucessoResultado();
            else
                return ResultadoDTO.ErroResultado("Erro ao remover o aluno.");

        }

        public ResultadoDTO BuscarAlunoPeloId(Guid id)
        {
            var aluno = _iAlunoRepository.BuscarPeloId(id);

            if (aluno is null)
                return ResultadoDTO.ErroResultado("Aluno não encontrado");

            return ResultadoDTO.SucessoResultado(aluno);

        }

        public ResultadoDTO ListarAlunos(string? nome = null, string? sobrenome = null, DateTime? dataDeNascimento = null, int page = 1, int itens = 50)
        {
            var listaAlunos = _iAlunoRepository.ListarAlunos(nome, sobrenome,dataDeNascimento, page, itens);

            var listaRetorno = new List<RetornoAlunoDTO>();
            
            foreach (var aluno in listaAlunos)
            {
                var alunoRetorno = new RetornoAlunoDTO
                {
                    DataNascimento = aluno.DataNascimento,
                    Documento = aluno.Documento,
                    Sobrenome = aluno.Sobrenome,
                    Nome = aluno.Nome,
                    Id = aluno.Id
                };

                if(aluno.IdMaterias is null)
                {
                    listaRetorno.Add(alunoRetorno);
                    continue;
                }

                foreach (var materia in aluno.IdMaterias)
                {
                    alunoRetorno.Materias ??= new List<RetornoMateriaDTO>();

                    var materiaRetorno = _iMateriaRepository.BuscarPeloId(materia);

                    if (materiaRetorno is null)
                        return ResultadoDTO.ErroResultado("Materia não encontrada!");

                    var materiaRetornoDTO = new RetornoMateriaDTO
                    {
                        Id = materiaRetorno.Id,
                        Nome = materiaRetorno.Nome
                    };

                    alunoRetorno.Materias.Add(materiaRetornoDTO);

                }

                listaRetorno.Add(alunoRetorno);

            }

            return ResultadoDTO.SucessoResultado(listaRetorno);

        }
    }
}
