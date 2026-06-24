using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Data.Repositories
{
    public class EstoqueMovimentoRepository : IEstoqueMovimentoRepository
    {
        private readonly ApplicationDbContext _context;

        public EstoqueMovimentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EstoqueMovimento?> ObterPorInsumoEUnidadeAsync(int insumoId, int unidadeId)
        {
            return await _context.EstoqueMovimentos
                .FirstOrDefaultAsync(s => s.InsumoId == insumoId && s.UnidadeId == unidadeId);
        }

        public async Task AdicionarAsync(EstoqueMovimento estoqueMovimento)
        {
            await _context.EstoqueMovimentos.AddAsync(estoqueMovimento);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(EstoqueMovimento estoqueMovimento)
        {
            _context.EstoqueMovimentos.Update(estoqueMovimento);
            await _context.SaveChangesAsync();
        }
    }
}