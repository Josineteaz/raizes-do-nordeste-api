using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IPedidoRepository
    {
        Task<Pedido?> ObterPorIdAsync(long id);
        Task<IEnumerable<Pedido>> ObterPorClienteIdAsync(int clienteId);
        Task AdicionarAsync(Pedido pedido);
        Task AtualizarAsync(Pedido pedido);
        Task<IEnumerable<Pedido>> ObterTodosAsync();
        Task<int> ObterUltimoNumeroPedidoPorUnidadeAsync(int unidadeId);
    }
}