using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

#nullable enable
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

            if (_turmaRepository.BuscarTurmaPeloNome(turma.Nome, turma.Id))
                return ResultadoDTO.ErroResultado("Já existe uma turma cadastrada com o nome informado!");

            var curso = _cursoRepository.BuscarCursoPeloId(turma.IdCurso);

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
                var materia = _materiaRepository.BuscaMateriaPeloId(id);

                if (materia is null)
                    return ResultadoDTO.ErroResultado("Materia informada não existe!");

            }

            if (!_turmaRepository.Cadastrar(turma))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar a turma!");

            return ResultadoDTO.SucessoResultado(turma);

        }

        public ResultadoDTO AtualizarTurma(Turma turma)
        {
            var turmaAtual = _turmaRepository.BuscarTurmaPeloId(turma.Id);

            if (turmaAtual is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada!");

            if (_turmaRepository.BuscarTurmaPeloNome(turma.Nome, turma.Id, true))
                return ResultadoDTO.ErroResultado("Já existe uma turma cadastrada com o nome informado!");

            var curso = _cursoRepository.BuscarCursoPeloId(turma.IdCurso);

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
            var turma = _turmaRepository.BuscarTurmaPeloId(id);

            if (turma is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada");

            if (_turmaRepository.VerificaSePossuiAlunoVinculado(id))
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
            var quantidadeMateriasJaCadastradas = _turmaRepository.BuscarQuantidadeMateriasCadastradas(idTurma);

            if ((idMaterias.Count + quantidadeMateriasJaCadastradas) > LimiteMaximoDeMateriasPorTurma)
                return ResultadoDTO.ErroResultado("Turma deve conter no máximo " + LimiteMaximoDeMateriasPorTurma + " matérias em sua grade curricular.");

            foreach (var id in idMaterias)
            {
                var materia = _materiaRepository.BuscaMateriaPeloId(id);

                if (materia is null)
                    return ResultadoDTO.ErroResultado("Matéria informada não existe!");

                if (_turmaRepository.VerificaVinculoMateriaComATurma(id, idTurma))
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
                if (!_turmaRepository.VerificaVinculoMateriaComATurma(id, idTurma))
                    return ResultadoDTO.ErroResultado("Matéria não vinculada com a turma informada!");

                //verificar se tem aluno matriculado a materia 
                if (_turmaRepository.VerificaAlunoMatriculadoMateria(id, idTurma))
                    return ResultadoDTO.ErroResultado("Existe aluno matriculado a matéria, favor cancelar a matricula!");
            }

            //Buscar materias já cadastradas para a turma informada
            var quantidadeMateriasJaCadastradas = _turmaRepository.BuscarQuantidadeMateriasCadastradas(idTurma);

            if ((quantidadeMateriasJaCadastradas - idMaterias.Count) < LimiteMinimoDeMateriasPorTurma)
                return ResultadoDTO.ErroResultado("Turma deve conter no mínimo " + LimiteMinimoDeMateriasPorTurma + " matérias em sua grade curricular.");

            if (!_turmaRepository.RemoverMaterias(idTurma, idMaterias))
                return ResultadoDTO.ErroResultado("Não foi possível remover as matérias!");

            return ResultadoDTO.SucessoResultado(idMaterias);

        }

        public ResultadoDTO BuscarAlunos(Guid idTurma)
        {
            var turma = _turmaRepository.BuscarTurmaPeloId(idTurma);

            if (turma is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada");

            var turmaAlunos = new RetornoTurmaDTO
            {
                DataInicio = turma.DataInicio,
                DataFim = turma.DataFim,
                IdCurso = turma.IdCurso,
                Nome = turma.Nome

            };

            foreach (var materia in turma.IdMaterias)
            {
                turmaAlunos.Materias ??= new List<RetornoMateriaDTO>();

                var materiaRetorno = _materiaRepository.BuscaMateriaPeloId(materia);

                if (materiaRetorno is null)
                    return ResultadoDTO.ErroResultado("Materia não encontrada!");

                var materiaRetornoDTO = new RetornoMateriaDTO
                {
                    Id = materiaRetorno.Id,
                    Nome = materiaRetorno.Nome
                };

                turmaAlunos.Materias.Add(materiaRetornoDTO);

            }

            var alunos = _turmaRepository.BuscarAlunos(idTurma);

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

                    var materiaRetorno = _materiaRepository.BuscaMateriaPeloId(materia);

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

        public ResultadoDTO BuscarTurmaPeloId(Guid id)
        {
            var turma = _turmaRepository.BuscarTurmaPeloId(id);

            if (turma is null)
                return ResultadoDTO.ErroResultado("Turma não encontrada");

            var turmaAlunos = new RetornoTurmaDTO
            {
                DataInicio = turma.DataInicio,
                DataFim = turma.DataFim,
                IdCurso = turma.IdCurso,
                Nome = turma.Nome

            };

            foreach (var materia in turma.IdMaterias)
            {
                turmaAlunos.Materias ??= new List<RetornoMateriaDTO>();

                var materiaRetorno = _materiaRepository.BuscaMateriaPeloId(materia);

                if (materiaRetorno is null)
                    return ResultadoDTO.ErroResultado("Materia não encontrada!");

                var materiaRetornoDTO = new RetornoMateriaDTO
                {
                    Id = materiaRetorno.Id,
                    Nome = materiaRetorno.Nome
                };

                turmaAlunos.Materias.Add(materiaRetornoDTO);

            }

            var alunos = _turmaRepository.BuscarAlunos(turma.Id);

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

                    var materiaRetorno = _materiaRepository.BuscaMateriaPeloId(materia);

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

            var turmas = _turmaRepository.ListarTurmas(nome, dataInicio, dataFim, page, itens);

            foreach (var turma in turmas)
            {
                var turmaAlunos = new RetornoTurmaDTO
                {
                    DataInicio = turma.DataInicio,
                    DataFim = turma.DataFim,
                    IdCurso = turma.IdCurso,
                    Nome = turma.Nome

                };

                foreach (var materia in turma.IdMaterias)
                {
                    turmaAlunos.Materias ??= new List<RetornoMateriaDTO>();

                    var materiaRetorno = _materiaRepository.BuscaMateriaPeloId(materia);

                    if (materiaRetorno is null)
                        return ResultadoDTO.ErroResultado("Materia não encontrada!");

                    var materiaRetornoDTO = new RetornoMateriaDTO
                    {
                        Id = materiaRetorno.Id,
                        Nome = materiaRetorno.Nome
                    };

                    turmaAlunos.Materias.Add(materiaRetornoDTO);

                }

                var alunos = _turmaRepository.BuscarAlunos(turma.Id);

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

                        var materiaRetorno = _materiaRepository.BuscaMateriaPeloId(materia);

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
