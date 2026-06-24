using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Data.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Categoria?> ObterPorIdAsync(int id)
        {
            return await _context.Categorias.FindAsync(id);
        }
        public async Task<IEnumerable<Categoria>> ObterTodasPorUnidadeAsync(int unidadeId)
        {
            return await _context.Categorias
                .Where(c => c.UnidadeId == unidadeId)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }
    }
}