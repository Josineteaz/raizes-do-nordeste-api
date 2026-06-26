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
        Task<IEnumerable<Pedido>> ObterTodosPaginadosAsync(int? unidadeId, string? canalPedido, int page, int limit);
        Task<int> ObterUltimoNumeroPedidoPorUnidadeAsync(int unidadeId);
        Task<bool> ExistePedidoPendentePorClienteAsync(int clienteId);

        Task<decimal> ObterTotalReservadoAsync(int produtoId, int unidadeId, DateTime limiteTempo, decimal quantidadeFicha);
    }
}