using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto?> ObterPorIdAsync(int id);

        Task<Produto?> ObterPorIdComSazonalidadesAsync(int id);

        Task<IEnumerable<Produto>> ObterPorCategoriaAsync(int unidadeId, int categoriaId, int page, int limit);

        Task<IEnumerable<Produto>> ObterTodosAsync(int unidadeId, int page, int limit);

        Task<Produto?> ObterPorNomeEUnidadeAsync(string nome, int unidadeId);

        Task AdicionarAsync(Produto produto);

        Task AtualizarAsync(Produto produto);
    }
}