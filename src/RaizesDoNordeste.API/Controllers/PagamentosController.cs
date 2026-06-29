using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;


namespace RaizesDoNordeste.API.Controllers
{
    [ApiController]
    [Route("api/pagamentos")]
    [Authorize]
    public class PagamentosController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IUserContext _userContext;

        public PagamentosController(IPagamentoService pagamentoService, IUserContext userContext)
        {
            _pagamentoService = pagamentoService;
            _userContext = userContext;
        }

        [HttpPost("simular")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagamentoResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Simular([FromBody] PagamentoRequestDto request)
        {
            var path = HttpContext.Request.Path.Value;
            var usuarioLogadoId = _userContext.ObterUsuarioLogadoId();

            if (usuarioLogadoId == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    error = "CREDENCIAIS_INVALIDAS",
                    message = "Token JWT ausente, expirado ou com assinatura inválida.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "REQUEST_INVALIDO",
                    message = "Dados de entrada inválidos, campos obrigatórios ausentes ou falha na validação do payload.",
                    details = new[] { new { field = "pedidoId", issue = "O ID do pedido é obrigatório." } },
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            try
            {
                var resultado = await _pagamentoService.ProcessarPagamentoSimuladoAsync(request);

                if (!resultado.Sucesso)
                {
                    if (resultado.Mensagem.Contains("não encontrado") || resultado.Mensagem.Contains("inexistente"))
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new
                        {
                            error = "PEDIDO_INEXISTENTE",
                            message = "Pedido enviado para processamento não foi localizado no sistema.",
                            details = Array.Empty<object>(),
                            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            path = path
                        });
                    }

                    if (resultado.Mensagem.Contains("status") && !resultado.Mensagem.Contains("AguardandoPagamento"))
                    {
                        return StatusCode(StatusCodes.Status409Conflict, new
                        {
                            error = "STATUS_CONFLITO",
                            message = "Pedido não está no estado 'AguardandoPagamento' ou já foi pago/cancelado anteriormente.",
                            details = Array.Empty<object>(),
                            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            path = path
                        });
                    }

                    return StatusCode(StatusCodes.Status402PaymentRequired, resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "ERRO_INTERNO",
                    message = "Ocorreu um erro inesperado ao processar o pagamento.",
                    details = new[] { new { field = "exception", issue = ex.Message } },
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
        }
    }
}