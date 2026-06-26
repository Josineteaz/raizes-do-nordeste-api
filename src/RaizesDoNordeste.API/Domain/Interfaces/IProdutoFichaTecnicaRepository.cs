using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IProdutoFichaTecnicaRepository
    {
        Task<IEnumerable<ProdutoFichaTecnica>> ObterPorProdutoIdAsync(int produtoId);
        Task<ProdutoFichaTecnica?> ObterItemEspecificoAsync(int produtoId, int insumoId);
        Task<bool> InsumoJaExisteNaFichaAsync(int produtoId, int insumoId);
        Task AdicionarAsync(ProdutoFichaTecnica itemFicha);
        Task AtualizarAsync(ProdutoFichaTecnica itemFicha);
        Task RemoverAsync(ProdutoFichaTecnica itemFicha);
    }
}