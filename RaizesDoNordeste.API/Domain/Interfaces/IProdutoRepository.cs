using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto?> ObterPorIdAsync(int id);

        Task<Produto?> ObterPorIdComSazonalidadesAsync(int id);

        Task<IEnumerable<Produto>> ObterPorCategoriaAsync(int categoriaId);

        Task<IEnumerable<Produto>> ObterTodosAsync();

        Task<Produto?> ObterPorNomeEUnidadeAsync(string nome, int unidadeId);

        Task AdicionarAsync(Produto produto);

        Task AtualizarAsync(Produto produto);
    }
}