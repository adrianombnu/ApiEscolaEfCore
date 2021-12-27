using ApiEscola.Entities;
using ApiEscola.Services;
using Microsoft.AspNetCore.Mvc;
using System;

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

        [HttpPost, Route("materias")]
        public IActionResult Cadastrar(MateriaDTO materiaDTO)
        {
            materiaDTO.Validar();

            if (!materiaDTO.Valido)
                return BadRequest(materiaDTO.Erros);

            try
            {
                var curso = new Materia(materiaDTO.Nome, materiaDTO.IdProfessor);

                return Created("", _materiaService.Cadastrar(curso));

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao criar a materia: " + ex.Message);
            }
        }

        [HttpGet, Route("materias")]
        public IActionResult Get()
        {
            return Ok(_materiaService.ListarMaterias());

        }
    }
}
