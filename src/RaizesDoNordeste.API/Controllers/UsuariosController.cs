using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IUserContext _userContext;

        public UsuariosController(IUsuarioService usuarioService, IUserContext userContext)
        {
            _usuarioService = usuarioService;
            _userContext = userContext;
        }

        [HttpGet]
        [Authorize(Roles = "AdministradorFranquia")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UsuarioResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterTodos([FromQuery] int page = 1, [FromQuery] int limit = 10)
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
            int? adminId = _userContext.ObterUsuarioLogadoId();

            var usuarios = await _usuarioService.ObterTodosPaginadoAsync(page, limit, adminId);

            Response.Headers.Append("X-Audit-Recorded", "true");
            Response.Headers.Append("X-Audit-Action", "ACESSO_SENSIVEL");
            return Ok(usuarios);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "AdministradorFranquia,Cliente")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
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

            var usuario = await _usuarioService.ObterPorIdAsync(id);

            if (usuario == null)
            {
                return NotFound(new
                {
                    error = "USUARIO_INEXISTENTE",
                    message = "Usuário não encontrado na base de dados.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = HttpContext.Request.Path.Value
                });
            }

            return Ok(usuario);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UsuarioResponseDto))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Criar([FromBody] UsuarioCreateDto usuarioDto)
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

            if (usuarioDto.Perfil == Perfil.Cliente && !usuarioDto.ConsentimentoLgpd)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "REGRA_LGPD_VIOLADA",
                    message = "O consentimento da LGPD é obrigatório para participar do programa de fidelidade.",
                    details = new[] { new { field = "consentimentoLgpd", issue = "Deve ser verdadeiro para Clientes." } },
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            try
            {
                var usuarioCriado = await _usuarioService.CriarUsuarioAsync(usuarioDto);
                return CreatedAtAction(nameof(ObterPorId), new { id = usuarioCriado.Id }, usuarioCriado);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "VIOLACAO_REGRA_NEGOCIO",
                    message = ex.Message,
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "AdministradorFranquia,GerenteUnidade,Cliente")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] UsuarioCreateDto usuarioDto)
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
                    message = "Dados de entrada inválidos.",
                    details = detalhesErro,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            try
            {
                var updated = await _usuarioService.AtualizarUsuarioAsync(id, usuarioDto);

                if (!updated)
                {
                    return NotFound(new
                    {
                        error = "USUARIO_INEXISTENTE",
                        message = "Usuário não encontrado.",
                        details = Array.Empty<object>(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        path = path
                    });
                }

                return Ok(new
                {
                    message = $"alteração efetuada do usuário n {id}",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                {
                    error = "VIOLACAO_REGRA_NEGOCIO",
                    message = ex.Message,
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "AdministradorFranquia, GerenteUnidade")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deletar(int id)
        {
            var path = HttpContext.Request.Path.Value;
            int? executorId = _userContext.ObterUsuarioLogadoId();
            var deletado = await _usuarioService.DesativarAsync(id, solicitouEsquecimentoLgpd: false, executorId);

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

            if (!deletado)
            {
                return NotFound(new
                {
                    error = "USUARIO_INEXISTENTE",
                    message = "Usuário não encontrado ou operação negada.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            return Ok(new
            {
                message = $"exclusão efetuada do usuário n {id}",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                path = path
            });
        }

        [HttpPost("{id:int}/excluir-lgpd")]
        [Authorize(Roles = "AdministradorFranquia, Cliente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExcluirDefinitivoLgpd(int id)
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

            int? executorId = _userContext.ObterUsuarioLogadoId();

            var executado = await _usuarioService.DesativarAsync(id, solicitouEsquecimentoLgpd: true, executorId);

            if (!executado)
            {
                return NotFound(new
                {
                    error = "USUARIO_INEXISTENTE",
                    message = "Usuário não encontrado ou operação negada.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            return Ok(new
            {
                message = $"Cliente anonimado por LGPD",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                path = path
            });
        }
    }
}