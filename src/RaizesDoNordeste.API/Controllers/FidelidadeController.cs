using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.API.Application.Interfaces;

namespace RaizesDoNordeste.API.Controllers
{
    [ApiController]
    [Route("api/fidelidade")]
    [Authorize]
    public class FidelidadeController : ControllerBase
    {
        private readonly IFidelidadeService _fidelidadeService;
        private readonly IUserContext _userContext;

        public FidelidadeController(IFidelidadeService fidelidadeService, IUserContext userContext)
        {
            _fidelidadeService = fidelidadeService;
            _userContext = userContext;
        }

        [HttpGet("saldo")]
        [Authorize(Roles = "Cliente,AdministradorFranquia")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ObterSaldo()
        {
            var path = HttpContext.Request.Path.Value;
            var usuarioLogadoId = _userContext.ObterUsuarioLogadoId();

            if (usuarioLogadoId == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    error = "CREDENCIAIS_INVALIDAS",
                    message = "Token inválido ou expirado.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
            if (!User.IsInRole("Cliente"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    error = "ACESSO_NEGADO",
                    message = "Usuário autenticado não possui o perfil de permissão necessário para esta consulta.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
            int clienteIdFinal = usuarioLogadoId.Value;

            try
            {
                var pontos = await _fidelidadeService.ObterPontosPorClienteAsync(clienteIdFinal);

                return Ok(new
                {
                    clienteId = clienteIdFinal,
                    saldoPontos = pontos
                });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "REQUEST_INVALIDO",
                    message = ex.Message,
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
        }

        [HttpGet("movimentacoes/{clienteIdAlvo:int?}")]
        [Authorize(Roles = "Cliente,GerenteUnidade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ObterMovimentacoes(int? clienteIdAlvo, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var path = HttpContext.Request.Path.Value;
            var usuarioLogadoId = _userContext.ObterUsuarioLogadoId();

            if (usuarioLogadoId == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    error = "CREDENCIAIS_INVALIDAS",
                    message = "Token inválido ou expirado.",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            if (page < 1) page = 1;
            if (limit < 1 || limit > 50) limit = 10;

            int clienteIdFinal;

            if (User.IsInRole("Cliente"))
            {
                clienteIdFinal = usuarioLogadoId.Value;
            }
            
            else
            {
                if (!clienteIdAlvo.HasValue || clienteIdAlvo.Value <= 0)
                {
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                    {
                        error = "CLIENTE_REQUERIDO",
                        message = "Como Gerente, você deve informar o ID de um cliente válido na URL para consultar o extrato.",
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }

                clienteIdFinal = clienteIdAlvo.Value;
            }

            var movimentacoes = await _fidelidadeService.ObterMovimentacoesPorClienteAsync(clienteIdFinal, page, limit);

            return Ok(movimentacoes);
        }

    }
}