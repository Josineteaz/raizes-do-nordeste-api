using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Domain.Entities;
using System.Threading.Tasks;

namespace RaizesDoNordeste.API.Application.Interfaces
{
    public interface IFidelidadeService
    {

        Task<int> ObterPontosPorClienteAsync(int clienteId);

        Task<FidelidadeResultadoDto> ProcessarResgateAsync(ResgatePontosDto resgateDto);

        Task<IEnumerable<FidelidadeMovimento>> ObterMovimentacoesPorClienteAsync(int clienteId, int page, int limit);
    }
}