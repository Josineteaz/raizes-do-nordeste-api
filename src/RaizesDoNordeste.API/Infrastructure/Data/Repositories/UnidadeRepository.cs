using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Data.Repositories
{
    public class UnidadeRepository : IUnidadeRepository
    {
        private readonly ApplicationDbContext _context;

        public UnidadeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unidade?> ObterPorIdAsync(int id)
        {
            return await _context.Unidades.FindAsync(id);
        }

        public async Task<IEnumerable<Unidade>> ObterTodasAsync()
        {
            return await _context.Unidades.ToListAsync();
        }

        public async Task AdicionarAsync(Unidade unidade)
        {
            await _context.Unidades.AddAsync(unidade);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Unidade unidade)
        {
            _context.Unidades.Update(unidade);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteCnpjAsync(string cnpj)
        {
            return await _context.Unidades.AnyAsync(u => u.Cnpj == cnpj);
        }

        public async Task<IEnumerable<Produto>> ObterProdutosPorUnidadeAsync(int unidadeId)
        {
            return await _context.Produtos
                .Where(p => p.UnidadeId == unidadeId && p.Ativo)
                .ToListAsync();
        }
    }
}