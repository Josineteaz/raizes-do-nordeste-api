using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Data.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Produto?> ObterPorIdAsync(int id)
        {
            return await _context.Produtos
                    .Include(p => p.ProdutoSazonalidades) 
                    .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Produto?> ObterPorIdComSazonalidadesAsync(int id)
        {
            return await _context.Produtos
                .Include(p => p.ProdutoSazonalidades)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Produto>> ObterPorCategoriaAsync(int unidadeId, int categoriaId, int page, int limit)
        {
            return await _context.Produtos
                .Include(p => p.ProdutoSazonalidades)
                .Where(p => p.UnidadeId == unidadeId && p.CategoriaId == categoriaId && p.Ativo)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produto>> ObterTodosAsync(int unidadeId, int page, int limit)
        {
            return await _context.Produtos
                .Include(p => p.ProdutoSazonalidades)
                .Where(p => p.UnidadeId == unidadeId && p.Ativo)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Produto?> ObterPorNomeEUnidadeAsync(string nome, int unidadeId)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(p => p.UnidadeId == unidadeId &&
                                          p.Nome.ToLower() == nome.ToLower());
        }

        public async Task AdicionarAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }
    }
}