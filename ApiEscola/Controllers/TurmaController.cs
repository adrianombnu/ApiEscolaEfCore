using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Services;
using Microsoft.AspNetCore.Mvc;
using System;

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

        /*
        [HttpPost, Route("turmas")]
        public IActionResult Cadastrar(TurmaDTO turmaDTO)
        {
            turmaDTO.Validar();

            if (!turmaDTO.Valido)
                return BadRequest(turmaDTO.Erros);

            try
            {
                var gui = Guid.NewGuid();
                var curso = new Turma(turmaDTO.Nome, turmaDTO.DataInicio, turmaDTO.DataFim, turmaDTO.Alunos, turmaDTO.Materias, gui);

                return Created("", _turmaService.Cadastrar(curso));
                
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao criar a turma: " + ex.Message);
            }
        }
        */

        [HttpGet, Route("turmas")]
        public IActionResult Get()
        {
            return Ok(_turmaService.ListarTurmas());

        }
    }
}
