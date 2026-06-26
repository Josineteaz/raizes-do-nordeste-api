using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IInsumoRepository
    {
        Task<Insumo?> ObterPorIdAsync(int id);
        Task<IEnumerable<Insumo>> ObterTodosPorUnidadeAsync(int unidadeId);
        Task AdicionarAsync(Insumo insumo);
        Task AtualizarAsync(Insumo insumo);
        Task<Insumo?> ObterPorNomeEUnidadeAsync(string nome, int unidadeId);
    }
}