using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Domain.Interfaces;

public class AuditoriaService : IAuditoriaService
{
    private readonly IAuditoriaRepository _auditoriaRepository;
    private static readonly string[] AcoesValidas = { "ACESSO_SENSIVEL", "ANONIMIZACAO_LGPD", "LOGIN_FALHA" };

    public AuditoriaService(IAuditoriaRepository auditoriaRepository)
    {
        _auditoriaRepository = auditoriaRepository;
    }

    public async Task<IEnumerable<AuditoriaResponseDto>> ObterLogsAuditadosAsync(string? recurso, string? acao, int page, int limit)
    {
        if (!string.IsNullOrEmpty(acao) && !AcoesValidas.Contains(acao))
        {
            throw new ArgumentException($"A ação '{acao}' não é uma operação de auditoria reconhecida.");
        }

        if (page < 1) page = 1;
        if (limit < 1 || limit > 50) limit = 10;

        var logsEntidade = await _auditoriaRepository.ObterLogsFiltradosPaginadosAsync(recurso, acao, page, limit);

        return logsEntidade.Select(l => new AuditoriaResponseDto
        {
            Id = l.Id,
            UnidadeId = l.UnidadeId,
            UsuarioId = l.UsuarioId,
            DataAcao = l.DataAcao,
            Acao = l.Acao,
            Entidade = l.Entidade,
            RegistroId = l.RegistroId,
            Descricao = l.Descricao
        });
    }
}