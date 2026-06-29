using RaizesDoNordeste.API.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaizesDoNordeste.API.Application.Interfaces
{
    public interface IUnidadeService
    {
        Task<IEnumerable<UnidadeResponseDto>> ObterTodasAsync();

        Task<UnidadeResponseDto?> ObterPorIdAsync(int id);

        Task<IEnumerable<ProdutoResponseDto>> ObterCardapioPorUnidadeAsync(int unidadeId);

        Task<UnidadeResponseDto> CriarUnidadeAsync(UnidadeCreateDto unidadeDto);

        Task<bool> AtualizarUnidadeAsync(int id, UnidadeCreateDto unidadeDto);

        Task<bool> DesativarUnidadeAsync(int id);
    }
}