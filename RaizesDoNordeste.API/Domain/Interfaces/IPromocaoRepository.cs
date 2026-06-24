using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IPromocaoRepository
    {
        Task<IEnumerable<Promocao>> ObterTodasAsync();
        Task<Promocao?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Promocao promocao);
        Task AtualizarAsync(Promocao promocao);
        Task DeletarAsync(Promocao promocao);
    }
}