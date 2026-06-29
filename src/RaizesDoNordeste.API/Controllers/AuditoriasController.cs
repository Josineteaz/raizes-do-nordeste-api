using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Interfaces;

[ApiController]
[Route("api/auditorias")]
[Authorize(Roles = "AdministradorFranquia")]
public class AuditoriasController : ControllerBase
{
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUserContext _userContext;

    public AuditoriasController(IAuditoriaService auditoriaService, IUserContext userContext)
    {
        _auditoriaService = auditoriaService;
        _userContext = userContext;
    }

    [HttpGet("logs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ObterLogs(
        [FromQuery] string? recurso,
        [FromQuery] string? acao,
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

        try
        {
            var resultado = await _auditoriaService.ObterLogsAuditadosAsync(recurso, acao, page, limit);
            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new {
                error = "AUDITORIA_INVALIDA",
                message = ex.Message,
                details = Array.Empty<object>(),
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                path = path

            });
        }
    }
}