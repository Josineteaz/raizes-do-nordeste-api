using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IFidelidadeRepository
    {
        Task<IEnumerable<FidelidadeMovimento>> ObterHistoricoPorClienteAsync(int clienteId);
        Task AdicionarMovimentacaoAsync(FidelidadeMovimento movimento);
    }
}