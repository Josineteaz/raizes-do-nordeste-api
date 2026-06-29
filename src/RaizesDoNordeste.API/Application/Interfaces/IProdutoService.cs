using System.Collections.Generic;
using System.Threading.Tasks;
using RaizesDoNordeste.API.Application.DTOs;

namespace RaizesDoNordeste.API.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoResponseDto>> ObterTodosAsync(int unidadeId, int page = 1, int limit = 10);

        Task<IEnumerable<ProdutoResponseDto>> ObterPorCategoriaAsync(int unidadeId, int categoriaId, int page = 1, int limit = 10);

        Task<ProdutoResponseDto?> ObterPorIdAsync(int id);

        Task<ProdutoResponseDto> CriarProdutoAsync(ProdutoCreateDto produtoDto);

        Task<bool> AtualizarProdutoAsync(int id, ProdutoCreateDto produtoDto);

        Task<bool> RemoverProdutoDaUnidadeAsync(int produtoId, int unidadeId);
    }
}