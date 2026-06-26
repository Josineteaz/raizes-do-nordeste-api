using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Data.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly ApplicationDbContext _context;

        public PagamentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Pagamento pagamento)
        {
            await _context.Set<Pagamento>().AddAsync(pagamento);
        }

        public async Task<Pagamento?> ObterPorIdAsync(long id)
        {
            return await _context.Set<Pagamento>()
                .Include(p => p.Pedido)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pagamento?> ObterPorPedidoIdAsync(long pedidoId)
        {
            return await _context.Set<Pagamento>()
                .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
        }
    }
}