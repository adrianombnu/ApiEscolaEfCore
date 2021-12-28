using ApiEscola.DTOs;
using ApiEscola.Entities;
using ApiEscola.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ApiEscola.Controllers
{
    [ApiController, Route("[controller]")]
    public class CursoController : ControllerBase
    {
        private readonly CursoService _cursoService;

        public CursoController(CursoService cursoService)
        {
            _cursoService = cursoService;
        }
        
        [HttpPost, Route("cursos")]
        public IActionResult Cadastrar(CursoDTO cursoDTO)
        {
            cursoDTO.Validar();

            if (!cursoDTO.Valido)
                return BadRequest(cursoDTO.Erros);

            try
            {
                var curso = new Curso(cursoDTO.Nome, cursoDTO.Descricao);

                return Created("", _cursoService.Cadastrar(curso));

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao cadastrar o curso: " + ex.Message);
            }
        }

        [HttpPut, Route("cursos")]
        public IActionResult Atualizar(Guid id, CursoDTO cursoDTO)
        {
            cursoDTO.Validar();

            if (!cursoDTO.Valido)
                return BadRequest(cursoDTO.Erros);

            try
            {
                var curso = new Curso(cursoDTO.Nome, cursoDTO.Descricao,id);

                var result = _cursoService.AtualizarCurso(curso);

                if (!result.Sucesso)
                    return BadRequest(result);
                else
                    return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao atualizar curso : " + ex.Message);
            }

        }

        [HttpDelete, Route("{id}/cursos")]
        public IActionResult RemoverCurso(Guid id)
        {
            var result = _cursoService.RemoverCurso(id);

            if (!result.Sucesso)
                return BadRequest(result);
            else
                return Ok(result);

        }

        [HttpGet, Route("{id}/cursos")]
        public IActionResult Get(Guid id)
        {
            return Ok(_cursoService.BuscaCursoPeloId(id));

        }

        [HttpGet, Route("cursos")]
        public IActionResult FiltrarCursos([FromQuery] string? nome, [FromQuery] string? descricao, [FromQuery] int page = 1, [FromQuery] int itens = 50)
        {
            return Ok(_cursoService.ListarCursos(nome, descricao, page, itens));

        }



    }
}
