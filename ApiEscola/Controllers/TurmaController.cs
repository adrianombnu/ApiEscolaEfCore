using ApiEscolaEfCore.DTOs;
using ApiEscolaEfCore.Entities;
using ApiEscolaEfCore.Services;
using Dominio.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

#nullable enable
namespace ApiEscolaEfCore.Controllers
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

        [HttpDelete, Route("{id}/turmas")]
        public IActionResult RemoverTurma(Guid id)
        {
            var result = _turmaService.RemoverTurma(id);

            if (!result.Sucesso)
                return BadRequest(result);
            else
                return Ok(result);

        }

        [HttpPost, Route("{idTurma}/materias")]
        public IActionResult AdicionarMaterias(Guid idTurma, TurmaMateriaDTO turmaMateriaDTO)
        {
            turmaMateriaDTO.Validar();

            if (!turmaMateriaDTO.Valido)
                return BadRequest(turmaMateriaDTO.Erros);

            try
            {
                return Created("", _turmaService.AdicionarMaterias(idTurma, turmaMateriaDTO.IdMaterias));

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao adicionar matéria: " + ex.Message);
            }
        }

        [HttpDelete, Route("{idTurma}/materias")]
        public IActionResult RemoverMaterias(Guid idTurma, TurmaMateriaDTO turmaMateriaDTO)
        {
            turmaMateriaDTO.Validar();

            if (!turmaMateriaDTO.Valido)
                return BadRequest(turmaMateriaDTO.Erros);

            try
            {
                return Created("", _turmaService.RemoverMaterias(idTurma, turmaMateriaDTO.IdMaterias));

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao remover materia: " + ex.Message);
            }

        }

        [HttpPut, Route("{idTurma}/turmas")]
        public IActionResult Atualizar(Guid idTurma, AtualizarTurmaDTO turmaDTO)
        {
            turmaDTO.Validar();

            if (!turmaDTO.Valido)
                return BadRequest(turmaDTO.Erros);

            try
            {
                var turma = new Turma(turmaDTO.Nome, turmaDTO.DataInicio, turmaDTO.DataFim, null, idTurma, turmaDTO.IdCurso);
                                        
                var result = _turmaService.AtualizarTurma(turma);

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
        /*

        [HttpGet, Route("{idTurma}/turmas/alunos")]
        public IActionResult BuscarAlunos(Guid idTurma)
        {
            return Ok(_turmaService.BuscarAlunos(idTurma));

        }
        */


        [HttpGet, Route("{id}/turmas")]
        public IActionResult Get(Guid id)
        {
            return Ok(_turmaService.BuscarTurmaPeloId(id));

        }

        [HttpGet, Route("turmas")]
        public IActionResult FiltrarTurmas([FromQuery] string? nome, [FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim, int page = 1, [FromQuery] int itens = 50)
        {
            return Ok(_turmaService.ListarTurmas(nome, dataInicio, dataFim, page, itens));

        }
    }
}
