using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IEstoqueMovimentoRepository
    {
        Task<EstoqueMovimento?> ObterPorInsumoEUnidadeAsync(int insumoId, int unidadeId);
        Task AdicionarAsync(EstoqueMovimento estoqueMovimento);
        Task AtualizarAsync(EstoqueMovimento estoqueMovimento);
    }
}