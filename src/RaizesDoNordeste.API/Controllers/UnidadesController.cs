using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;

namespace RaizesDoNordeste.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/unidades")]
    public class UnidadesController : ControllerBase
    {
        private readonly IUnidadeService _unidadeService;

        public UnidadesController(IUnidadeService unidadeService)
        {
            _unidadeService = unidadeService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodas([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1 || limit > 50) limit = 10;

            var unidades = await _unidadeService.ObterTodasAsync();
            var unidadesPaginadas = unidades.Skip((page - 1) * limit).Take(limit);

            return Ok(unidadesPaginadas);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var unidade = await _unidadeService.ObterPorIdAsync(id);
            if (unidade == null)
            {
                return NotFound(new
                {
                    error = "UNIDADE_INEXISTENTE",
                    message = "Unidade não encontrada.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = HttpContext.Request.Path.Value
                });
            }

            return Ok(unidade);
        }

        [HttpPost]
        [Authorize(Roles = "AdministradorFranquia")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Criar([FromBody] UnidadeCreateDto unidadeDto)
        {
            var path = HttpContext.Request.Path.Value;

            if (!ModelState.IsValid)
            {
                var detalhesErro = ModelState.Keys
                    .SelectMany(key => ModelState[key].Errors.Select(e => new { field = key, issue = e.ErrorMessage }))
                    .ToList();

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "REQUEST_INVALIDO",
                    message = "Dados de entrada inválidos.",
                    details = detalhesErro,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            try
            {
                var novaUnidade = await _unidadeService.CriarUnidadeAsync(unidadeDto);
                return CreatedAtAction(nameof(ObterPorId), new { id = novaUnidade.Id }, novaUnidade);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new
                {
                    error = "CONFLITO_CADASTRO",
                    message = ex.Message,
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] UnidadeCreateDto unidadeDto)
        {
            var path = HttpContext.Request.Path.Value;

            if (!User.IsInRole("AdministradorFranquia"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    error = "ACESSO_PROIBIDO",
                    message = "Seu perfil de usuário não tem permissão para alterar dados de uma unidade.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            if (!ModelState.IsValid)
            {
                var detalhesErro = ModelState.Keys
                    .SelectMany(key => ModelState[key].Errors.Select(e => new { field = key, issue = e.ErrorMessage }))
                    .ToList();

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "REQUEST_INVALIDO",
                    message = "Dados de entrada inválidos.",
                    details = detalhesErro,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            try
            {
                var atualizado = await _unidadeService.AtualizarUnidadeAsync(id, unidadeDto);
                if (!atualizado)
                {
                    return NotFound(new
                    {
                        error = "UNIDADE_INEXISTENTE",
                        message = "Unidade não encontrada para atualização.",
                        details = Array.Empty<object>(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }

                return Ok(new
                {
                    message = $"alteração efetuada da unidade n {id}",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new
                {
                    error = "CONFLITO_ATUALIZACAO",
                    message = ex.Message,
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "AdministradorFranquia")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DesativarUnidade(int id)
        {
            var desativada = await _unidadeService.DesativarUnidadeAsync(id);
            if (!desativada)
            {
                return NotFound(new
                {
                    error = "UNIDADE_INEXISTENTE",
                    message = "Unidade não encontrada.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = HttpContext.Request.Path.Value
                });
            }

            return NoContent();
        }
    }
}