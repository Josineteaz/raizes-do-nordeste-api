using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Usuario user);
    }
}