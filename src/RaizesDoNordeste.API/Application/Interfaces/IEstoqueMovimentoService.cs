using System.Threading.Tasks;
using RaizesDoNordeste.API.Application.DTOs;

namespace RaizesDoNordeste.API.Application.Interfaces
{
    public interface IEstoqueMovimentoService
    {
        Task<int> ObterSaldoAtualAsync(int produtoId, int unidadeId);

        Task<bool> TemEstoqueSuficienteAsync(int produtoId, int unidadeId, int quantidade);

        Task<bool> DeduzirEstoqueAsync(int produtoId, int unidadeId, int quantidade);

        Task<bool> RegistrarMovimentacaoAsync(EstoqueMovimentoDto movimentoDto);
    }
}