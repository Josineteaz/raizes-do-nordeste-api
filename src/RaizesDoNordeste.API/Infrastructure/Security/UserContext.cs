using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using RaizesDoNordeste.API.Application.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Security
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? ObterUsuarioLogadoId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var usuarioIdClaim = user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                 ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? user?.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(usuarioIdClaim) || !int.TryParse(usuarioIdClaim, out int id))
            {
                return null;
            }

            return id;
        }

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }
    }
}