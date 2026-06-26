using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;
using RaizesDoNordeste.API.Infrastructure.Data;

namespace RaizesDoNordeste.API.Infrastructure.Repositories
{
    public class ProdutoFichaTecnicaRepository : IProdutoFichaTecnicaRepository
    {
        private readonly ApplicationDbContext _context;

        public ProdutoFichaTecnicaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProdutoFichaTecnica>> ObterPorProdutoIdAsync(int produtoId)
        {
            return await _context.ProdutoFichasTecnicas
                .Where(f => f.ProdutoId == produtoId)
                .ToListAsync();
        }

        public async Task<ProdutoFichaTecnica?> ObterItemEspecificoAsync(int produtoId, int insumoId)
        {
            return await _context.ProdutoFichasTecnicas
                .FirstOrDefaultAsync(f => f.ProdutoId == produtoId && f.InsumoId == insumoId);
        }

        public async Task<bool> InsumoJaExisteNaFichaAsync(int produtoId, int insumoId)
        {
            return await _context.ProdutoFichasTecnicas
                .AnyAsync(f => f.ProdutoId == produtoId && f.InsumoId == insumoId);
        }

        public async Task AdicionarAsync(ProdutoFichaTecnica itemFicha)
        {
            await _context.ProdutoFichasTecnicas.AddAsync(itemFicha);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(ProdutoFichaTecnica itemFicha)
        {
            _context.ProdutoFichasTecnicas.Update(itemFicha);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(ProdutoFichaTecnica itemFicha)
        {
            _context.ProdutoFichasTecnicas.Remove(itemFicha);
            await _context.SaveChangesAsync();
        }
    }
}