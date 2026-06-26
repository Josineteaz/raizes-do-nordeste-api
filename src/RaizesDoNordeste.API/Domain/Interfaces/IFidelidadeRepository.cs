using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IFidelidadeRepository
    {
        Task<IEnumerable<FidelidadeMovimento>> ObterHistoricoPorClientePaginadoAsync(int clienteId, int page, int limit);
        Task AdicionarMovimentacaoAsync(FidelidadeMovimento movimento);
    }
}