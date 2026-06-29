using System.Threading.Tasks;
using RaizesDoNordeste.API.Application.DTOs;

namespace RaizesDoNordeste.API.Application.Interfaces
{
    public interface IPagamentoService
    {
        Task<PagamentoResponseDto> ProcessarPagamentoSimuladoAsync(PagamentoRequestDto request);
    }
}