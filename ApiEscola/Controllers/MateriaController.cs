using ApiEscola.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiEscola.Controllers
{
    [ApiController, Route("[controller]")]
    public class MateriaController : ControllerBase
    {
        private readonly MateriaService _materiaService;

        public MateriaController(MateriaService materiaService)
        {
            _materiaService = materiaService;
        }

        [HttpGet, Route("materias")]
        public IActionResult Get()
        {
            return Ok(_materiaService.ListarMaterias());

        }
    }
}
