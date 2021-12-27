using ApiEscola.DTOs;
using ApiEscola.Services;
using Microsoft.AspNetCore.Mvc;
using ApiEscola.Entities;
using System;

namespace ApiEscola.Controllers
{
    [ApiController, Route("[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly ProfessorService _professorService;

        public ProfessorController(ProfessorService professorService)
        {
            _professorService = professorService;
        }

        [HttpPost, Route("professor")]
        public IActionResult Cadastrar(ProfessorDTO professorDTO)
        {
            professorDTO.Validar();

            if (!professorDTO.Valido)
                return BadRequest(professorDTO.Erros);

            try
            {
                var gui = Guid.NewGuid();
                var professor = new Professor(professorDTO.Nome, professorDTO.Sobrenome, professorDTO.DataNascimento, professorDTO.Documento);

                return Created("", _professorService.Cadastrar(professor));

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao criar o professor: " + ex.Message);
            }
        }

        [HttpPut, Route("professores")]
        public IActionResult Atualizar(Guid id, ProfessorDTO professorDTO)
        {
            professorDTO.Validar();

            if (!professorDTO.Valido)
                return BadRequest(professorDTO.Erros);

            try
            {
                var professor = new Professor(professorDTO.Nome, professorDTO.Sobrenome, professorDTO.DataNascimento, professorDTO.Documento,id);

                var result = _professorService.AtualizarProfessor(professor);

                if (!result.Sucesso)
                    return BadRequest(result);
                else
                    return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao atualizar os dados do professor: " + ex.Message);
            }

        }

        [HttpDelete, Route("{id}/professores")]
        public IActionResult RemoverProfessor(Guid id)
        {
            var result = _professorService.RemoverProfessor(id);

            if (!result.Sucesso)
                return BadRequest(result);
            else
                return Ok(result);

        }

        [HttpGet, Route("professores")]
        public IActionResult Get()
        {
            return Ok(_professorService.ListarProfessores());

        }
    }
}
