using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using System.IO;
using System.Security.Claims;

namespace RaizesDoNordeste.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserContext _userContext;

        public AuthController(IAuthService authService, IUserContext userContext)
        {
            _authService = authService;
            _userContext = userContext;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
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
                    message = "Dados de entrada inválidos para a autenticação.",
                    details = detalhesErro,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            var loginResult = await _authService.AutenticarAsync(loginDto);

            if (loginResult == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new
                {
                    error = "CREDENCIAIS_INVALIDAS",
                    message = "E-mail ou senha incorretos.",
                    details = Array.Empty<object>(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    path = path
                });
            }

            return Ok(new
            {
                accessToken = loginResult.Token,
                tokenType = "Bearer",
                role = loginResult.Role
            });
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var path = HttpContext.Request.Path.Value;
            int? usuarioId = _userContext.ObterUsuarioLogadoId();

            if (usuarioId == null)
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

            await _authService.RegistrarLogoutAsync(usuarioId);

            return Ok(new { message = "Logout realizado com sucesso." });
        }
    }
}