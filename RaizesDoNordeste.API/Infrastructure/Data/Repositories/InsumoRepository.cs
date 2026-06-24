using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Data.Repositories
{
    public class InsumoRepository : IInsumoRepository
    {
        private readonly ApplicationDbContext _context;

        public InsumoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Insumo?> ObterPorIdAsync(int id)
        {
            return await _context.Insumos
                .Include(i => i.Unidade)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Insumo>> ObterTodosPorUnidadeAsync(int unidadeId)
        {
            return await _context.Insumos
                .Where(i => i.UnidadeId == unidadeId)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Insumo insumo)
        {
            await _context.Insumos.AddAsync(insumo);
            await _context.SaveChangesAsync();
        }

        public async Task System_AtualizarAsync(Insumo insumo)
        {
            _context.Insumos.Update(insumo);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Insumo insumo)
        {
            _context.Insumos.Update(insumo);
            await _context.SaveChangesAsync();
        }
        public async Task<Insumo?> ObterPorNomeEUnidadeAsync(string nome, int unidadeId)
        {
            return await _context.Insumos
                .FirstOrDefaultAsync(i => i.Nome.ToLower() == nome.ToLower() && i.UnidadeId == unidadeId && i.Ativo);
        }
    }
}