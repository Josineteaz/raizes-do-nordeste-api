using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;

namespace RaizesDoNordeste.API.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterTodos(
            [FromQuery] int? unidadeId,
            [FromQuery] int? categoriaId,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            if (!unidadeId.HasValue)
            {
                return BadRequest(new
                {
                    error = "PARAMETRO_REQUERIDO",
                    message = "O parâmetro 'unidadeId' é obrigatório para realizar a busca.",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                });
            }

            if (page < 1) page = 1;
            if (limit < 1 || limit > 50) limit = 10;

            if (categoriaId.HasValue)
            {
                var mapProdutosPorCategoria = await _produtoService.ObterPorCategoriaAsync(unidadeId.Value, categoriaId.Value, page, limit);
                return Ok(mapProdutosPorCategoria);
            }

            var produtos = await _produtoService.ObterTodosAsync(unidadeId.Value, page, limit);
            return Ok(produtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);
            if (produto == null)
            {
                return NotFound(new
                {
                    error = "PRODUTO_INEXISTENTE",
                    message = "Produto não encontrado no catálogo.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = HttpContext.Request.Path.Value
                });
            }

            return Ok(produto);
        }

        [HttpPost]
        [Authorize(Roles = "AdministradorFranquia, GerenteUnidade")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Criar([FromBody] ProdutoCreateDto produtoDto)
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
                var produtoCriado = await _produtoService.CriarProdutoAsync(produtoDto);
                return CreatedAtAction(nameof(ObterPorId), new { id = produtoCriado.Id }, produtoCriado);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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
        [Authorize(Roles = "AdministradorFranquia, GerenteUnidade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ProdutoCreateDto produtoDto)
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
                var atualizado = await _produtoService.AtualizarProdutoAsync(id, produtoDto);
                if (!atualizado)
                {
                    return NotFound(new
                    {
                        error = "PRODUTO_INEXISTENTE",
                        message = "Produto não encontrado para atualização.",
                        details = Array.Empty<object>(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }

                return Ok(new
                {
                    message = $"alteração efetuada do produto n {id}",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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
        [HttpDelete("{produtoId:int}/unidades/{unidadeId:int}")]
        [Authorize(Roles = "AdministradorFranquia, GerenteUnidade")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RemoverDaUnidade(int produtoId, int unidadeId)
        {
            var path = HttpContext.Request.Path.Value;

            try
            {
                var removido = await _produtoService.RemoverProdutoDaUnidadeAsync(produtoId, unidadeId);
                if (!removido)
                {
                    return NotFound(new
                    {
                        error = "VINCULO_INEXISTENTE",
                        message = "Vínculo entre produto e unidade não encontrado.",
                        details = Array.Empty<object>(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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
    }
}