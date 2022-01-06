using ApiEscolaEfCore.Entities;
using ApiEscolaEfCore.Services;
using Dominio.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

#nullable enable
namespace ApiEscolaEfCore.Controllers
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
                var materia = new Materia(materiaDTO.Nome, materiaDTO.IdProfessor);

                return Created("", _materiaService.Cadastrar(materia));

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao cadastrar a materia: " + ex.Message);
            }
        }

        [HttpPut, Route("materias")]
        public IActionResult Atualizar(Guid id, MateriaDTO materiaDTO)
        {
            materiaDTO.Validar();

            if (!materiaDTO.Valido)
                return BadRequest(materiaDTO.Erros);

            try
            {
                var materia = new Materia(materiaDTO.Nome, materiaDTO.IdProfessor, id);

                var result = _materiaService.AtualizarMateria(materia);

                if (!result.Sucesso)
                    return BadRequest(result);
                else
                    return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao atualizar os dados da matéria: " + ex.Message);
            }

        }

        [HttpDelete, Route("{id}/materias")]
        public IActionResult RemoverMateria(Guid id)
        {
            var result = _materiaService.RemoverMateria(id);

            if (!result.Sucesso)
                return BadRequest(result);
            else
                return Ok(result);

        }

        [HttpGet, Route("{id}/materias")]
        public IActionResult Get(Guid id)
        {
            return Ok(_materiaService.BuscarPeloId(id));

        }

        [HttpGet, Route("materias")]
        public IActionResult FiltrarMaterias([FromQuery] string? nome, int page = 1, [FromQuery] int itens = 50)
        {
            return Ok(_materiaService.ListarMaterias(nome, page, itens));

        }
    }
}
