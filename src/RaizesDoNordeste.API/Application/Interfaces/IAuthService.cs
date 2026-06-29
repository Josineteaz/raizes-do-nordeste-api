using System.Threading.Tasks;
using RaizesDoNordeste.API.Application.DTOs;

namespace RaizesDoNordeste.API.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> AutenticarAsync(LoginDto loginDto);

        Task RegistrarLogoutAsync(int? usuarioId);
    }
}