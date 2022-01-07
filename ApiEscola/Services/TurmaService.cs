using ApiEscolaEfCore.DTOs;
using ApiEscolaEfCore.Entities;
using ApiEscolaEfCore.Repository;
using Dominio;
using Dominio.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

#nullable enable
namespace ApiEscolaEfCore.Services
{
    public class TurmaService
    {
        private readonly TurmaRepository _turmaRepository;
        private readonly ITurmaRepository _iTurmaRepository;
        private readonly CursoRepository _cursoRepository;
        private readonly ICursoRepository _iCursoRepository;
        private readonly MateriaRepository _materiaRepository;
        private readonly IMateriaRepository _iMateriaRepository;
        private readonly IConfiguration _configuration;

        public TurmaService(TurmaRepository turmaRepository,
                            ITurmaRepository iTurmaRepository,
                            CursoRepository cursoRepository,
                            ICursoRepository iCursoRepository,
                            MateriaRepository materiaRepository,
                            IMateriaRepository iMateriaRepository,
                            IConfiguration configuration)
        {
            _turmaRepository = turmaRepository;
            _iTurmaRepository = iTurmaRepository;
            _cursoRepository = cursoRepository;
            _iCursoRepository = iCursoRepository;
            _materiaRepository = materiaRepository;
            _iMateriaRepository = iMateriaRepository;
            _configuration = configuration;
        }


        public ResultadoDTO Cadastrar(Turma turma)
        {
            var LimiteMinimoDeMateriasPorTurma = _configuration.GetValue<int>("LimiteMinimoDeMateriasPorTurma");
            var LimiteMaximoDeMateriasPorTurma = _configuration.GetValue<int>("LimiteMaximoDeMateriasPorTurma");

            if (_iTurmaRepository.BuscarTurmaPeloNome(turma.Nome, turma.Id))
                return ResultadoDTO.ErroResultado("Já existe uma turma cadastrada com o nome informado!");

            var curso = _iCursoRepository.BuscarPeloId(turma.IdCurso);

            if (curso is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado");

            if (turma.DataInicio.Date < DateTime.Now.Date)
                return ResultadoDTO.ErroResultado("Data de inicio não pode ser menor que " + DateTime.Now.Date.ToString("dd/MM/yyyy"));

            if (turma.DataFim.Date < turma.DataInicio.Date)
                return ResultadoDTO.ErroResultado("Data fim não pode ser menor que " + turma.DataInicio.Date.ToString("dd/MM/yyyy"));

            if (turma.IdMaterias.Count < LimiteMinimoDeMateriasPorTurma)
                return ResultadoDTO.ErroResultado("Turma deve conter no minímo " + LimiteMinimoDeMateriasPorTurma + " matéria em sua grade curricular.");

            if (turma.IdMaterias.Count > LimiteMaximoDeMateriasPorTurma)
                return ResultadoDTO.ErroResultado("Turma deve conter no máximo " + LimiteMaximoDeMateriasPorTurma + " matérias em sua grade curricular.");

            foreach (var id in turma.IdMaterias)
            {
                var materia = _iMateriaRepository.BuscarPeloId(id);

                if (materia is null)
                    return ResultadoDTO.ErroResultado("Materia informada não existe!");

            }

            if (!_turmaRepository.Cadastrar(turma))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar a turma!");

            return ResultadoDTO.SucessoResultado(turma);

        }

        public ResultadoDTO AtualizarTurma(Turma turma)
        {
            var turmaAtual = _iTurmaRepository.BuscarPeloId(turma.Id);

            if (turmaAtual is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada!");

            if (_iTurmaRepository.BuscarTurmaPeloNome(turma.Nome, turma.Id, true))
                return ResultadoDTO.ErroResultado("Já existe uma turma cadastrada com o nome informado!");

            var curso = _iCursoRepository.BuscarPeloId(turma.IdCurso);

            if (curso is null)
                return ResultadoDTO.ErroResultado("Curso não encontrado");

            if (turma.DataInicio.Date < DateTime.Now.Date)
                return ResultadoDTO.ErroResultado("Data de inicio não pode ser menor que " + DateTime.Now.Date.ToString("dd/MM/yyyy"));

            if (turma.DataFim.Date < turma.DataInicio.Date)
                return ResultadoDTO.ErroResultado("Data fim não pode ser menor que " + turma.DataInicio.Date.ToString("dd/MM/yyyy"));

            if (!_turmaRepository.AtualizarTurma(turma))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar a turma!");

            return ResultadoDTO.SucessoResultado(turma);

        }

        public ResultadoDTO RemoverTurma(Guid id)
        {
            var turma = _iTurmaRepository.BuscarPeloId(id);

            if (turma is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada");

            if (_iTurmaRepository.VerificaSePossuiAlunoVinculado(id))
                return ResultadoDTO.ErroResultado("Existem alunos vinculados a esta turma, favor remove-los");

            if (_turmaRepository.RomoverTurma(id))
                return ResultadoDTO.SucessoResultado();
            else
                return ResultadoDTO.ErroResultado("Erro ao remover a turma");

        }

        public ResultadoDTO AdicionarMaterias(Guid idTurma, List<Guid> idMaterias)
        {
            var LimiteMaximoDeMateriasPorTurma = _configuration.GetValue<int>("LimiteMaximoDeMateriasPorTurma");

            //Buscar materias já cadastradas para a turma informada
            var quantidadeMateriasJaCadastradas = _iTurmaRepository.BuscarQuantidadeMateriasCadastradas(idTurma);

            if ((idMaterias.Count + quantidadeMateriasJaCadastradas) > LimiteMaximoDeMateriasPorTurma)
                return ResultadoDTO.ErroResultado("Turma deve conter no máximo " + LimiteMaximoDeMateriasPorTurma + " matérias em sua grade curricular.");

            foreach (var id in idMaterias)
            {
                var materia = _iMateriaRepository.BuscarPeloId(id);

                if (materia is null)
                    return ResultadoDTO.ErroResultado("Matéria informada não existe!");

                if (_iTurmaRepository.VerificaVinculoMateriaComATurma(id, idTurma))
                    return ResultadoDTO.ErroResultado("Matéria já cadastrada para a turma informada!");

            }

            if (!_turmaRepository.AdicionarMaterias(idTurma, idMaterias))
                return ResultadoDTO.ErroResultado("Não foi possível adicionar as matérias!");

            return ResultadoDTO.SucessoResultado(idMaterias);

        }

        public ResultadoDTO RemoverMaterias(Guid idTurma, List<Guid> idMaterias)
        {
            var LimiteMinimoDeMateriasPorTurma = _configuration.GetValue<int>("LimiteMinimoDeMateriasPorTurma");

            foreach (var id in idMaterias)
            {
                if (!_iTurmaRepository.VerificaVinculoMateriaComATurma(id, idTurma))
                    return ResultadoDTO.ErroResultado("Matéria não vinculada com a turma informada!");

                //verificar se tem aluno matriculado a materia 
                if (_iTurmaRepository.VerificaAlunoMatriculadoMateria(id, idTurma))
                    return ResultadoDTO.ErroResultado("Existe aluno matriculado a matéria, favor cancelar a matricula!");
            }

            //Buscar materias já cadastradas para a turma informada
            var quantidadeMateriasJaCadastradas = _iTurmaRepository.BuscarQuantidadeMateriasCadastradas(idTurma);

            if ((quantidadeMateriasJaCadastradas - idMaterias.Count) < LimiteMinimoDeMateriasPorTurma)
                return ResultadoDTO.ErroResultado("Turma deve conter no mínimo " + LimiteMinimoDeMateriasPorTurma + " matérias em sua grade curricular.");

            if (!_turmaRepository.RemoverMaterias(idTurma, idMaterias))
                return ResultadoDTO.ErroResultado("Não foi possível remover as matérias!");

            return ResultadoDTO.SucessoResultado(idMaterias);

        }
        

        public ResultadoDTO BuscarTurmaPeloId(Guid id)
        {
            var turma = _iTurmaRepository.BuscarPeloId(id);

            if (turma is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada");

            var turmaAlunos = new RetornoTurmaDTO
            {
                DataInicio = turma.DataInicio,
                DataFim = turma.DataFim,
                IdCurso = turma.IdCurso,
                Nome = turma.Nome,
                Id = turma.Id

            };

            foreach (var materia in turma.IdMaterias)
            {
                turmaAlunos.Materias ??= new List<RetornoMateriaDTO>();

                var materiaRetorno = _iMateriaRepository.BuscarPeloId(materia);

                if (materiaRetorno is null)
                    return ResultadoDTO.ErroResultado("Materia não encontrada!");

                var materiaRetornoDTO = new RetornoMateriaDTO
                {
                    Id = materiaRetorno.Id,
                    Nome = materiaRetorno.Nome
                };

                turmaAlunos.Materias.Add(materiaRetornoDTO);

            }

            var alunos = _iTurmaRepository.BuscarAlunos(turma.Id);

            if (alunos is null)
                return ResultadoDTO.SucessoResultado(turmaAlunos);

            foreach (var aluno in alunos)
            {
                turmaAlunos.Alunos ??= new List<RetornoAlunoDTO>();

                var alunoRetorno = new RetornoAlunoDTO
                {
                    DataNascimento = aluno.DataNascimento,
                    Documento = aluno.Documento,
                    Sobrenome = aluno.Sobrenome,
                    Nome = aluno.Nome,
                    Id = aluno.Id
                };

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

                turmaAlunos.Alunos.Add(alunoRetorno);
            }

            return ResultadoDTO.SucessoResultado(turmaAlunos);

        }

        public ResultadoDTO ListarTurmas(string? nome = null, DateTime? dataInicio = null, DateTime? dataFim = null, int page = 1, int itens = 50)
        {
            //return _turmaRepository.ListarTurmas(nome, dataInicio, dataFim, page, itens);

            var listaTurmas = new List<RetornoTurmaDTO>();

            var turmas = _iTurmaRepository.ListarTurmas(nome, dataInicio, dataFim, page, itens);
            
            foreach (var turma in turmas)
            {
                var turmaAlunos = new RetornoTurmaDTO
                {
                    DataInicio = turma.DataInicio,
                    DataFim = turma.DataFim,
                    IdCurso = turma.IdCurso,
                    Nome = turma.Nome,
                    Id = turma.Id

                };
                
                //22041292722

                foreach (var materia in turma.IdMaterias)
                {
                    turmaAlunos.Materias ??= new List<RetornoMateriaDTO>();

                    var materiaRetorno = _iMateriaRepository.BuscarPeloId(materia);

                    if (materiaRetorno is null)
                        return ResultadoDTO.ErroResultado("Materia não encontrada!");

                    var materiaRetornoDTO = new RetornoMateriaDTO
                    {
                        Id = materiaRetorno.Id,
                        Nome = materiaRetorno.Nome
                    };

                    turmaAlunos.Materias.Add(materiaRetornoDTO);

                }

                var alunos = _iTurmaRepository.BuscarAlunos(turma.Id);

                if (alunos is null)
                {
                    listaTurmas.Add(turmaAlunos);
                    return ResultadoDTO.SucessoResultado(listaTurmas);

                }

                foreach (var aluno in alunos)
                {
                    turmaAlunos.Alunos ??= new List<RetornoAlunoDTO>();

                    var alunoRetorno = new RetornoAlunoDTO
                    {
                        DataNascimento = aluno.DataNascimento,
                        Documento = aluno.Documento,
                        Sobrenome = aluno.Sobrenome,
                        Nome = aluno.Nome,
                        Id = aluno.Id
                    };

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

                    turmaAlunos.Alunos.Add(alunoRetorno);
                }

                listaTurmas.Add(turmaAlunos);

            }

            return ResultadoDTO.SucessoResultado(listaTurmas);

        }

    }

}
