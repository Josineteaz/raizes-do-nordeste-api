using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Application.Services
{
    public class PromocaoService : IPromocaoService
    {
        private readonly IPromocaoRepository _promocaoRepository;

        public PromocaoService(IPromocaoRepository promocaoRepository)
        {
            _promocaoRepository = promocaoRepository;
        }

      
        public async Task<IEnumerable<PromocaoResponseDto>> ObterTodasAsync(int page = 1, int limit = 10)
        {
            var promocoes = await _promocaoRepository.ObterTodasAsync();
            var dataAtual = DateTime.UtcNow;

            return promocoes
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(p => MapearParaResponseDto(p, dataAtual))
                .ToList();
        }

        public async Task<PromocaoResponseDto?> ObterPorIdAsync(int id)
        {
            var promocao = await _promocaoRepository.ObterPorIdAsync(id);
            if (promocao == null) return null;

            return MapearParaResponseDto(promocao, DateTime.UtcNow);
        }

        public async Task<PromocaoResponseDto> CriarAsync(PromocaoDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (dto.DataInicio >= dto.DataFim)
            {
                throw new InvalidOperationException("A data de início da promoção deve ser anterior à data de término.");
            }

            var novaPromocao = new Promocao
            {
                UnidadeId = dto.UnidadeId,
                ProdutoId = dto.ProdutoId,
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                TipoDesconto = dto.TipoDesconto,
                ValorDesconto = dto.ValorDesconto,
                DataInicio = dto.DataInicio,
                DataFim = dto.DataFim
            };

            await _promocaoRepository.AdicionarAsync(novaPromocao);
            return MapearParaResponseDto(novaPromocao, DateTime.UtcNow);
        }

        public async Task<bool> AtualizarAsync(int id, PromocaoDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var promocao = await _promocaoRepository.ObterPorIdAsync(id);
            if (promocao == null) return false;

            if (dto.DataInicio >= dto.DataFim)
            {
                throw new InvalidOperationException("A data de início da promoção deve ser anterior à data de término.");
            }

            promocao.UnidadeId = dto.UnidadeId;
            promocao.ProdutoId = dto.ProdutoId;
            promocao.Nome = dto.Nome;
            promocao.Descricao = dto.Descricao;
            promocao.TipoDesconto = dto.TipoDesconto;
            promocao.ValorDesconto = dto.ValorDesconto;
            promocao.DataInicio = dto.DataInicio;
            promocao.DataFim = dto.DataFim;

            await _promocaoRepository.AtualizarAsync(promocao);
            return true;
        }

        public async Task<bool> DeletarAsync(int id)
        {
            var promocao = await _promocaoRepository.ObterPorIdAsync(id);
            if (promocao == null) return false;

            await _promocaoRepository.DeletarAsync(promocao);
            return true;
        }

       
        public async Task<IEnumerable<PromocaoResponseDto>> ObterPorUnidadeAsync(int unidadeId, int page = 1, int limit = 10)
        {
            var todasPromocoes = await _promocaoRepository.ObterTodasAsync();
            var dataAtual = DateTime.UtcNow;

            return todasPromocoes
                .Where(p => p.UnidadeId == unidadeId && dataAtual >= p.DataInicio && dataAtual <= p.DataFim)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(p => MapearParaResponseDto(p, dataAtual))
                .ToList();
        }

        #region Mapeamentos Auxiliares

        private static PromocaoResponseDto MapearParaResponseDto(Promocao promocao, DateTime dataReferencia)
        {
            return new PromocaoResponseDto
            {
                Id = promocao.Id,
                UnidadeId = promocao.UnidadeId,
                ProdutoId = promocao.ProdutoId,
                Nome = promocao.Nome,
                Descricao = promocao.Descricao,
                TipoDesconto = promocao.TipoDesconto,
                ValorDesconto = promocao.ValorDesconto,
                DataInicio = promocao.DataInicio,
                DataFim = promocao.DataFim,
                EstaAtivaHoje = dataReferencia >= promocao.DataInicio && dataReferencia <= promocao.DataFim
            };
        }

        #endregion
    }
}