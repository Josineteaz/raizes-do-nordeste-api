using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IAuditoriaRepository
    {
        Task SalvarAuditoriaAsync(Auditoria auditoria);
        Task<IEnumerable<Auditoria>> ObterLogsFiltradosPaginadosAsync(string? entidade, string? acao, int page, int limit);
    }
}