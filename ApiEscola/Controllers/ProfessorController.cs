using ApiEscolaEfCore.DTOs;
using ApiEscolaEfCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApiEscolaEfCore.Entities;
using System;
using Dominio.Entities;

#nullable enable
namespace ApiEscolaEfCore.Controllers
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
                var professor = new Professor(professorDTO.Nome, professorDTO.Sobrenome, professorDTO.DataNascimento, professorDTO.Documento);

                return Created("", _professorService.Cadastrar(professor));

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao cadastrar o professor: " + ex.Message);
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
        public IActionResult FiltrarProfessores([FromQuery] string? nome, [FromQuery] string? sobrenome, [FromQuery] DateTime? dataDeNascimento, [FromQuery] string? documento, int page = 1, [FromQuery] int itens = 50)
        {
            return Ok(_professorService.ListarProfessores(nome, sobrenome, dataDeNascimento, documento, page, itens));

        }
    }
}
