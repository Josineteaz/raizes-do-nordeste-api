using System.Collections.Generic;
using System.Threading.Tasks;
using RaizesDoNordeste.API.Application.DTOs;

namespace RaizesDoNordeste.API.Application.Interfaces
{
    public interface IPromocaoService
    {
       
        Task<IEnumerable<PromocaoResponseDto>> ObterTodasAsync(int page = 1, int limit = 10);

        Task<PromocaoResponseDto?> ObterPorIdAsync(int id);

        Task<PromocaoResponseDto> CriarAsync(PromocaoDto dto);

        Task<bool> AtualizarAsync(int id, PromocaoDto dto);

        Task<bool> DeletarAsync(int id);

      
        Task<IEnumerable<PromocaoResponseDto>> ObterPorUnidadeAsync(int unidadeId, int page = 1, int limit = 10);
    }
}