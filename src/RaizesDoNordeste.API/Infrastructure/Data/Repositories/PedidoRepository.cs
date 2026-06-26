using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Enums;
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



        public async Task<IEnumerable<Pedido>> ObterTodosPaginadosAsync(int? unidadeId, string? canalPedido, int page, int limit)
        {
            IQueryable<Pedido> query = _context.Pedidos
                .AsNoTracking()
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto);

            if (unidadeId.HasValue)
            {
                query = query.Where(p => p.UnidadeId == unidadeId.Value);
            }

            if (!string.IsNullOrEmpty(canalPedido) && Enum.TryParse<CanalPedido>(canalPedido, true, out var canalEnum))
            {
                query = query.Where(p => p.CanalPedido == canalEnum);
            }

            return await query
                .OrderByDescending(p => p.DataPedido)
                .Skip((page - 1) * limit)
                .Take(limit)
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

        public async Task<bool> ExistePedidoPendentePorClienteAsync(int clienteId)
        {
            return await _context.Pedidos
                .AnyAsync(p => p.ClienteId == clienteId && p.Status == StatusPedido.AguardandoPagamento);
        }
        public async Task<decimal> ObterTotalReservadoAsync(int produtoId, int unidadeId, DateTime limiteTempo, decimal quantidadeFicha)
        {
            return await _context.Set<PedidoItem>()
                .Where(pi => pi.Pedido.UnidadeId == unidadeId &&
                             pi.Pedido.Status == StatusPedido.AguardandoPagamento &&
                             pi.Pedido.DataPedido >= limiteTempo)
                .SumAsync(pi => pi.Quantidade * quantidadeFicha);
        }
    }
}