using ApiEscolaEfCore.DTOs;
using ApiEscolaEfCore.Repository;
using Dominio;
using Dominio.Entities;
using System;

#nullable enable
namespace ApiEscolaEfCore.Services
{
    public class MateriaService
    {
        private readonly IMateriaRepository _iMateriaRepository;
        private readonly IMateriaRepositoryEfCore _iMateriaRepositoryEfCore;
        private readonly ProfessorRepository _professorRepository;
        private readonly IProfessorRepository _iProfessorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MateriaService(ProfessorRepository professorRepository,
                              IMateriaRepository iMateriaRepository,
                              IMateriaRepositoryEfCore iMateriaRepositoryEfCore,
                              IProfessorRepository iProfessorRepository,
                              IUnitOfWork unitOfWork)
        {
            _iMateriaRepository = iMateriaRepository;
            _iMateriaRepositoryEfCore = iMateriaRepositoryEfCore;
            _professorRepository = professorRepository;
            _iProfessorRepository = iProfessorRepository;
            _unitOfWork = unitOfWork;
        }

        public ResultadoDTO Cadastrar(Materia materia)
        {            
            /*if (_materiaRepository.BuscaMateriaPeloNome(materia.Nome))
                return ResultadoDTO.ErroResultado("Já existe uma materia cadastrada com o nome informado!");
            */

            if (_iMateriaRepository.VerificaSeMateriaJaCadastrada(materia.Nome, materia.Id, materia.IdProfessor))
                return ResultadoDTO.ErroResultado("Matéria já cadastrada para o professor informado!");

            var professor = _iProfessorRepository.BuscarPeloId(materia.IdProfessor);

            if (professor is null)
                return ResultadoDTO.ErroResultado("O professor informado não foi encontrado!");

            /*if (!_materiaRepository.Cadastrar(materia))
                return ResultadoDTO.ErroResultado("Não foi possível cadastrar a matéria!");
            */

            _iMateriaRepositoryEfCore.Incluir(materia);
            _unitOfWork.Commit();

            return ResultadoDTO.SucessoResultado(materia);

        }

        public ResultadoDTO AtualizarMateria(Materia materia)
        {
            var materiaAtual = _iMateriaRepository.BuscarPeloId(materia.Id);

            if (materiaAtual is null)
                return ResultadoDTO.ErroResultado("Matéria não encontrada!");

            if (_iMateriaRepository.VerificaSeMateriaJaCadastrada(materia.Nome, materia.Id, materia.IdProfessor, true))
                return ResultadoDTO.ErroResultado("Matéria já cadastrada para o professor informado!");

            var professor = _iProfessorRepository.BuscarPeloId(materia.IdProfessor);

            if (professor is null)
                return ResultadoDTO.ErroResultado("O prefessor informado não foi encontrado!");

            /*
                 if (!_materiaRepository.AtualizarMateria(materia))
                 return ResultadoDTO.ErroResultado("Matéria não pode ser atualizada!");
            */
            
            _iMateriaRepositoryEfCore.Atualizar(materia);

            return ResultadoDTO.SucessoResultado(materia);

        }

        public ResultadoDTO RemoverMateria(Guid id)
        {
            var materia = _iMateriaRepository.BuscarPeloId(id);

            if (materia is null)
                return ResultadoDTO.ErroResultado("Matéria não encontrada");

            if (_iMateriaRepository.VerificaSePossuiTurmaVinculada(id))
                return ResultadoDTO.ErroResultado("Matéria já está vinculada a uma turma, favor remover o vinculo");

            /*if (_materiaRepository.RomoverMateria(id))
                return ResultadoDTO.SucessoResultado();
            else
                return ResultadoDTO.ErroResultado("Erro ao remover a matéria");
            */
            _iMateriaRepositoryEfCore.Remover(materia);

            return ResultadoDTO.SucessoResultado();

        }

        public ResultadoDTO BuscarPeloId(Guid id)
        {
            var materia = _iMateriaRepository.BuscarPeloId(id);

            if (materia is null)
                return ResultadoDTO.ErroResultado("Matéria não encontrada");

            return ResultadoDTO.SucessoResultado(materia);

        }

        public ResultadoDTO ListarMaterias(string? nome = null, int page = 1, int itens = 50)
        {
            var listaMeterias = _iMateriaRepository.ListarMaterias(nome, page, itens);

            return ResultadoDTO.SucessoResultado(listaMeterias);

        }
    }
}
