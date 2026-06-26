using System.Collections.Generic;
using System.Threading.Tasks;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<Categoria?> ObterPorIdAsync(int id);
        Task<IEnumerable<Categoria>> ObterTodasPorUnidadeAsync(int unidadeId);
        Task AdicionarAsync(Categoria categoria);
        Task AtualizarAsync(Categoria categoria);
    }
}