using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;
using RaizesDoNordeste.API.Infrastructure.Data;

namespace RaizesDoNordeste.API.Infrastructure.Repositories
{
    public class FidelidadeRepository : IFidelidadeRepository
    {
        private readonly ApplicationDbContext _context;

        public FidelidadeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FidelidadeMovimento>> ObterHistoricoPorClienteAsync(int clienteId)
        {
            return await _context.FidelidadeMovimentos
                .Where(m => m.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task AdicionarMovimentacaoAsync(FidelidadeMovimento movimento)
        {
            await _context.FidelidadeMovimentos.AddAsync(movimento);
            await _context.SaveChangesAsync();
        }
    }
}