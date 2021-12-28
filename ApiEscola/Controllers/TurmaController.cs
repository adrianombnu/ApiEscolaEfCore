using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ApiEscola.Controllers
{
    [ApiController, Route("[controller]")]
    public class TurmaController : ControllerBase
    {
        private readonly TurmaService _turmaService;

        public TurmaController(TurmaService turmaService)
        {
            _turmaService = turmaService;
        }

        [HttpPost, Route("turmas")]
        public IActionResult Cadastrar(TurmaDTO turmaDTO)
        {
            turmaDTO.Validar();

            if (!turmaDTO.Valido)
                return BadRequest(turmaDTO.Erros);

            try
            {
                var turma = new Turma(turmaDTO.Nome, turmaDTO.DataInicio, turmaDTO.DataFim, turmaDTO.IdMaterias, turmaDTO.IdCurso);
                
                return Created("", _turmaService.Cadastrar(turma));
                
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao cadastrar a turma: " + ex.Message);
            }
        }

        [HttpGet, Route("{id}/turmas")]
        public IActionResult Get(Guid id)
        {
            return Ok(_turmaService.BuscaTurmaPeloId(id));

        }

        [HttpGet, Route("turmas")]
        public IActionResult FiltrarTurmas([FromQuery] string? nome, [FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim, int page = 1, [FromQuery] int itens = 50)
        {
            return Ok(_turmaService.ListarTurmas(nome, dataInicio, dataFim, page, itens));

        }
    }
}
