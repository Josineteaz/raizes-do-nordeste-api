using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Infrastructure.Security;


namespace RaizesDoNordeste.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly IUserContext _userContext;

        public PedidosController(IPedidoService pedidoService, IUserContext userContext)
        {
            _pedidoService = pedidoService;
            _userContext = userContext;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Criar([FromBody] PedidoCreateDto pedidoDto)
        {
            var path = HttpContext.Request.Path.Value;
            var usuarioLogadoId = _userContext.ObterUsuarioLogadoId();

            if (usuarioLogadoId == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    error = "CREDENCIAIS_INVALIDAS",
                    message = "Token de autenticação ausente, inválido ou expirado.",
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
                    message = "Dados de entrada inválidos para a criação do pedido.",
                    details = detalhesErro,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            var resultadoPedido = await _pedidoService.CriarPedidoAsync(pedidoDto);

            if (!resultadoPedido.Sucesso)
            {
                if (resultadoPedido.TipoErro == "Forbidden")
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        error = "ACESSO_NEGADO",
                        message = resultadoPedido.Mensagem,
                        details = Array.Empty<object>(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }
                if (resultadoPedido.TipoErro == "NotFound")
                {
                    return NotFound(new
                    {
                        error = "RECURSO_INEXISTENTE",
                        message = resultadoPedido.Mensagem,
                        details = Array.Empty<object>(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }

                if (resultadoPedido.TipoErro == "InsufficientStock")
                {
                    return StatusCode(StatusCodes.Status409Conflict, new
                    {
                        error = "CONFLITO_ESTOQUE",
                        message = resultadoPedido.Mensagem,
                        details = Array.Empty<object>(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "REGRA_NEGOCIO_VIOLADA",
                    message = resultadoPedido.Mensagem,
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = resultadoPedido.PedidoId },
                new
                {
                    pedidoId = resultadoPedido.PedidoId,
                    mensagem = "Pedido criado com sucesso! Aguardando o processamento do pagamento.",
                    dadosPedido = resultadoPedido.Dados
                }
            );
        }

        [HttpGet("{id:long}")]
        [Authorize(Roles = "Cliente,Atendente,Cozinheiro,GerenteUnidade,AdministradorFranquia")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(long id)
        {
            var path = HttpContext.Request.Path.Value;
            var usuarioLogadoId = _userContext.ObterUsuarioLogadoId();

            if (usuarioLogadoId == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    error = "CREDENCIAIS_INVALIDAS",
                    message = "Token de autenticação ausente, inválido ou expirado.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            var pedido = await _pedidoService.ObterPedidoPorIdAsync(id);

            if (pedido == null)
            {
                return NotFound(new
                {
                    error = "PEDIDO_INEXISTENTE",
                    message = $"Pedido com ID {id} não foi encontrado.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = HttpContext.Request.Path.Value
                });
            }

            return Ok(pedido);
        }

        [HttpGet]
        [Authorize(Roles = "Atendente,Cozinheiro,GerenteUnidade,AdministradorFranquia")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterTodos([FromQuery] string? canalPedido, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var path = HttpContext.Request.Path.Value;
            var usuarioLogadoId = _userContext.ObterUsuarioLogadoId();

            if (usuarioLogadoId == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    error = "CREDENCIAIS_INVALIDAS",
                    message = "Token de autenticação ausente, inválido ou expirado.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            if (page < 1) page = 1;
            if (limit < 1 || limit > 50) limit = 10;

            int? unidadeId = null;
            var unidadeClaim = User.FindFirst("UnidadeId")?.Value;
            if (int.TryParse(unidadeClaim, out var id))
            {
                unidadeId = id;
            }

            var pedidos = await _pedidoService.ObterPedidosAsync(unidadeId, canalPedido, page, limit);
            return Ok(pedidos);
        }

        [HttpGet("unidade/{unidadeId}/pagos")]
        [Authorize(Roles = "Cozinheiro,GerenteUnidade")]
        [ProducesResponseType(typeof(IEnumerable<PedidoFilaProducaoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterPedidosPagosPorUnidade(
            [FromRoute] int unidadeId,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var path = HttpContext.Request.Path.Value;
            var usuarioLogadoId = _userContext.ObterUsuarioLogadoId();

            if (usuarioLogadoId == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    error = "CREDENCIAIS_INVALIDAS",
                    message = "Token de autenticação ausente, inválido ou expirado.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            if (page < 1) page = 1;
            if (limit < 1 || limit > 50) limit = 10;

            var pedidos = await _pedidoService.ObterPedidosPagosPorUnidadeAsync(unidadeId, page, limit);

            return Ok(pedidos);
        }

        [HttpGet("unidade/{unidadeId}/prontos")]
        [Authorize(Roles = "Atendente,GerenteUnidade")]
        [ProducesResponseType(typeof(IEnumerable<PedidoFilaBalcaoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterPedidosProntosPorUnidade(
            [FromRoute] int unidadeId,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var path = HttpContext.Request.Path.Value;
            var usuarioLogadoId = _userContext.ObterUsuarioLogadoId();

            if (usuarioLogadoId == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    error = "CREDENCIAIS_INVALIDAS",
                    message = "Token de autenticação ausente, inválido ou expirado.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            if (page < 1) page = 1;
            if (limit < 1 || limit > 50) limit = 10;

            var pedidos = await _pedidoService.ObterPedidosProntosPorUnidadeAsync(unidadeId, page, limit);

            return Ok(pedidos);
        }


        [HttpPatch("{id:long}/status")]
        [Authorize(Roles = "Atendente,Cozinheiro,GerenteUnidade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AtualizarStatus(long id, [FromBody] PedidoStatusUpdateDto statusDto)
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
                    message = "Dados de entrada inválidos para atualização de status.",
                    details = detalhesErro,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            if (!Enum.IsDefined(typeof(StatusPedido), statusDto.NovoStatus))
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "STATUS_INVALIDO",
                    message = "O status informado não é válido.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            if (statusDto.NovoStatus == StatusPedido.Pago)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "OPERACAO_NEGADA",
                    message = "Não é permitido alterar o status de um pedido para 'Pago' manualmente. Isso é controlado pelo sistema de pagamentos.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            var pedido = await _pedidoService.ObterPedidoPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound(new
                {
                    error = "PEDIDO_INEXISTENTE",
                    message = $"Pedido com ID {id} não foi encontrado.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            var unityClaim = User.FindFirst("UnidadeId")?.Value;
            if (!int.TryParse(unityClaim, out int userUnityId) || pedido.UnidadeId != userUnityId)
            {
                return Forbid();
            }

            var statusAntigo = pedido.Status.ToString();
            var novoStatus = statusDto.NovoStatus.ToString();

            try
            {
                var updated = await _pedidoService.AtualizarStatusAsync(id, statusDto.NovoStatus);

                if (!updated)
                {
                    return NotFound(new
                    {
                        error = "PEDIDO_INEXISTENTE",
                        message = $"Erro ao salvar a atualização do pedido com ID {id}.",
                        details = Array.Empty<object>(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }

                return Ok(new
                {
                    message = $"Status do pedido nº {id} alterado de {statusAntigo} para {novoStatus}",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                });
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