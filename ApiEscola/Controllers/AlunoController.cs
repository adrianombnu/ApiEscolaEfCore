using ApiEscolaEfCore.Entities;
using ApiEscolaEfCore.Services;
using Dominio.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

#nullable enable
namespace ApiEscolaEfCore.Controllers
{
    [ApiController, Route("[controller]")]
    public class AlunoController : ControllerBase
    {
        private readonly AlunoService _alunoService;

        public AlunoController(AlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        [HttpPost, Route("{idTurma}/alunos")]
        public IActionResult Cadastrar(Guid idTurma, AlunoDTO alunoDTO)
        {
            alunoDTO.Validar();

            if (!alunoDTO.Valido)
                return BadRequest(alunoDTO.Erros);

            try
            {
                var aluno = new Aluno(alunoDTO.Nome, alunoDTO.Sobrenome, alunoDTO.DataNascimento, alunoDTO.Documento, alunoDTO.IdMaterias);

                return Created("", _alunoService.Cadastrar(idTurma, aluno));

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

        [HttpGet, Route("alunos")]
        public IActionResult FiltrarAlunos([FromQuery] string? nome, [FromQuery] string? sobrenome, [FromQuery] DateTime? dataDeNascimento, [FromQuery] int page = 1, [FromQuery] int itens = 50)
        {
            return Ok(_alunoService.ListarAlunos(nome, sobrenome, dataDeNascimento, page, itens));

        }

    }
}
