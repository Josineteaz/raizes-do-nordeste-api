using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Data.Repositories
{
    public class PromocaoRepository : IPromocaoRepository
    {
        private readonly ApplicationDbContext _context;

        public PromocaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Promocao?> ObterPorIdAsync(int id)
        {
            return await _context.Promocoes.FindAsync(id);
        }

        public async Task<IEnumerable<Promocao>> ObterTodasAsync()
        {
            return await _context.Promocoes.ToListAsync();
        }

        public async Task AdicionarAsync(Promocao promocao)
        {
            await _context.Promocoes.AddAsync(promocao);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Promocao promocao)
        {
            _context.Promocoes.Update(promocao);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(Promocao promocao)
        {
            _context.Promocoes.Remove(promocao);
            await _context.SaveChangesAsync();
        }
    }
}