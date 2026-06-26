using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IPagamentoRepository
    {
        Task AdicionarAsync(Pagamento pagamento);
        Task<Pagamento?> ObterPorIdAsync(long id);
        Task<Pagamento?> ObterPorPedidoIdAsync(long pedidoId);
    }
}