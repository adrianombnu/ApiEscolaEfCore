using ApiEscola.Entities;
using ApiEscola.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ApiEscola.Controllers
{
    [ApiController, Route("[controller]")]
    public class AlunoController : ControllerBase
    {
        private readonly AlunoService _alunoService;

        public AlunoController(AlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        [HttpPost, Route("turmas")]
        public IActionResult Cadastrar(AlunoDTO alunoDTO)
        {
            alunoDTO.Validar();

            if (!alunoDTO.Valido)
                return BadRequest(alunoDTO.Erros);

            try
            {
                var aluno = new Aluno(alunoDTO.Nome, alunoDTO.Sobrenome, alunoDTO.DataNascimento, alunoDTO.Documento, alunoDTO.IdMaterias, alunoDTO.IdTurma);

                return Created("", _alunoService.Cadastrar(aluno));

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao cadastrar aluno: " + ex.Message);
            }
        }

        [HttpGet, Route("{id}/alunos")]
        public IActionResult Get(Guid id)
        {
            return Ok(_alunoService.BuscarAlunoPeloId(id));

        }

        [HttpDelete, Route("{id}/alunos")]
        public IActionResult RemoverAluno(Guid id)
        {
            var result = _alunoService.RemoverAluno(id);

            if (!result.Sucesso)
                return BadRequest(result);
            else
                return Ok(result);

        }

    }
}
