using ApiEscola.Entities;
using System.Collections.Generic;

namespace ApiEscola.Services
{
    public class MateriaService
    {
        private readonly MateriaRepository _materiaRepository;

        public MateriaService(MateriaRepository repository)
        {
            _materiaRepository = repository;
        }
        public IEnumerable<Materia> ListarMaterias()
        {
            return _materiaRepository.ListarMaterias();

        }
    }
}
