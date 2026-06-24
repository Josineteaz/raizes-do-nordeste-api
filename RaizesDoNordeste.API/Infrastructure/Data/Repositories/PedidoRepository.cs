using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Data.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ApplicationDbContext _context;

        public PedidoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Pedido?> ObterPorIdAsync(long id)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pedido>> ObterPorClienteIdAsync(int clienteId)
        {
            return await _context.Pedidos
                .AsNoTracking()
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Pedido pedido)
        {
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Pedido>> ObterTodosAsync()
        {
            return await _context.Pedidos
                .AsNoTracking()
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .ToListAsync();
        }

        public async Task<int> ObterUltimoNumeroPedidoPorUnidadeAsync(int unidadeId)
        {
            var ultimoNumero = await _context.Pedidos
                .AsNoTracking()
                .Where(p => p.UnidadeId == unidadeId)
                .MaxAsync(p => (int?)p.NumPedidoUnidade);

            return ultimoNumero ?? 0;
        }
    }
}