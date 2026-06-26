using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Gera um hash seguro e único para a senha informada
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            // Compara a senha digitada no login com o hash salvo no banco
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}