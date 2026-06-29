using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;

namespace RaizesDoNordeste.API.Controllers
{
    [Authorize(Roles = "AdministradorFranquia")]
    [ApiController]
    [Route("api/promocoes")]
    public class PromocoesController : ControllerBase
    {
        private readonly IPromocaoService _promocaoService;

        public PromocoesController(IPromocaoService promocaoService)
        {
            _promocaoService = promocaoService;
        }

        [HttpGet("unidade/{unidadeId:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorUnidade(int unidadeId, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1 || limit > 50) limit = 10;

            var promocoes = await _promocaoService.ObterPorUnidadeAsync(unidadeId, page, limit);
            return Ok(promocoes);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterTodas([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1 || limit > 50) limit = 10;

            var promocoes = await _promocaoService.ObterTodasAsync(page, limit);
            return Ok(promocoes);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var promocao = await _promocaoService.ObterPorIdAsync(id);
            if (promocao == null)
            {
                return NotFound(new
                {
                    error = "PROMOCAO_INEXISTENTE",
                    message = "Promoção não encontrada na base de dados.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = HttpContext.Request.Path.Value
                });
            }

            return Ok(promocao);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Criar([FromBody] PromocaoDto dto)
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
                    message = "Não foi possível processar a requisição devido a erros de validação.",
                    details = detalhesErro,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            try
            {
                var novaPromocao = await _promocaoService.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = novaPromocao.Id }, novaPromocao);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "REGRA_NEGOCIO_VIOLADA",
                    message = ex.Message,
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] PromocaoDto dto)
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
                    message = "Não foi possível processar a requisição devido a erros de validação.",
                    details = detalhesErro,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            try
            {
                var atualizado = await _promocaoService.AtualizarAsync(id, dto);
                if (!atualizado)
                {
                    return NotFound(new
                    {
                        error = "PROMOCAO_INEXISTENTE",
                        message = "Promoção não encontrada para atualização.",
                        details = Array.Empty<object>(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "REGRA_NEGOCIO_VIOLADA",
                    message = ex.Message,
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deletar(int id)
        {
            var path = HttpContext.Request.Path.Value;

            try
            {
                var deletado = await _promocaoService.DeletarAsync(id);
                if (!deletado)
                {
                    return NotFound(new
                    {
                        error = "PROMOCAO_INEXISTENTE",
                        message = "Promoção não encontrada para exclusão.",
                        details = Array.Empty<object>(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new
                {
                    error = "CONFLITO_EXCLUSAO",
                    message = ex.Message,
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
        }
    }
}