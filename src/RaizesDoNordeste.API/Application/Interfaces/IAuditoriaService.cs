using System.Collections.Generic;
using System.Threading.Tasks;
using RaizesDoNordeste.API.Application.DTOs;

namespace RaizesDoNordeste.API.Domain.Interfaces
{
    public interface IAuditoriaService
    {

        Task<IEnumerable<AuditoriaResponseDto>> ObterLogsAuditadosAsync(
            string? recurso,
            string? acao,
            int page,
            int limit);
    }
}