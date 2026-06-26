using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorIdAsync(int id);
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<IEnumerable<Usuario>> ObterTodosAsync();

        Task<IEnumerable<Usuario>> ObterTodosPaginadoAsync(int page, int limit);

        Task AdicionarAsync(Usuario usuario);
        Task AtualizarAsync(Usuario usuario);
        Task DesativarAsync(int id);
    }
}