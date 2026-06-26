using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IUnidadeRepository
    {
        Task<Unidade?> ObterPorIdAsync(int id);
        Task<IEnumerable<Unidade>> ObterTodasAsync();
        Task AdicionarAsync(Unidade unidade);
        Task AtualizarAsync(Unidade unidade);
        Task<bool> ExisteCnpjAsync(string cnpj);
        Task<IEnumerable<Produto>> ObterProdutosPorUnidadeAsync(int unidadeId);
    }
}