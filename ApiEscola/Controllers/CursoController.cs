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
                return BadRequest("Erro ao criar o curso: " + ex.Message);
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

        [HttpGet, Route("cursos")]
        public IActionResult Get()
        {
            return Ok(_cursoService.ListarCursos());

        }



    }
}
