using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Data.Repositories
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SalvarAuditoriaAsync(Auditoria auditoria)
        {
            await _context.Auditorias.AddAsync(auditoria);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Auditoria>> ObterLogsFiltradosPaginadosAsync(string? entidade, string? acao, int page, int limit)
        {
            IQueryable<Auditoria> query = _context.Auditorias;
            if (!string.IsNullOrWhiteSpace(entidade))
            {
                query = query.Where(a => a.Entidade.ToLower() == entidade.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(acao))
            {
                query = query.Where(a => a.Acao == acao);
            }

            return await query
                .OrderByDescending(a => a.DataAcao)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }
    }
}