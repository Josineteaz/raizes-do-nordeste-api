using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Enums;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Application.Services
{
    public class UnidadeService : IUnidadeService
    {
        private readonly IUnidadeRepository _unidadeRepository;

        public UnidadeService(IUnidadeRepository unidadeRepository)
        {
            _unidadeRepository = unidadeRepository;
        }

        public async Task<IEnumerable<UnidadeResponseDto>> ObterTodasAsync()
        {
            var unidades = await _unidadeRepository.ObterTodasAsync();

            return unidades
                .Where(u => u.Ativo)
                .Select(MapearParaResponseDto)
                .ToList();
        }

        public async Task<UnidadeResponseDto?> ObterPorIdAsync(int id)
        {
            var unidade = await _unidadeRepository.ObterPorIdAsync(id);

            if (unidade == null || !unidade.Ativo) return null;

            return MapearParaResponseDto(unidade);
        }

        public async Task<IEnumerable<ProdutoResponseDto>> ObterCardapioPorUnidadeAsync(int unidadeId)
        {
            var unity = await _unidadeRepository.ObterPorIdAsync(unidadeId);
            if (unity == null || !unity.Ativo) return Enumerable.Empty<ProdutoResponseDto>();

            var produtos = await _unidadeRepository.ObterProdutosPorUnidadeAsync(unidadeId);

            return produtos.Select(p => new ProdutoResponseDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Preco = p.Preco,
                UrlImagem = p.UrlImagem,
                Ativo = p.Ativo
            }).ToList();
        }

        public async Task<UnidadeResponseDto> CriarUnidadeAsync(UnidadeCreateDto unidadeDto)
        {
            if (unidadeDto == null) throw new ArgumentNullException(nameof(unidadeDto));

            var cnpjExistente = await _unidadeRepository.ExisteCnpjAsync(unidadeDto.Cnpj);
            if (cnpjExistente)
            {
                throw new InvalidOperationException("Já existe uma unidade cadastrada com este CNPJ.");
            }

            var novaUnidade = new Unidade
            {
                Cnpj = unidadeDto.Cnpj,
                Nome = unidadeDto.Nome,
                Telefone = unidadeDto.Telefone,
                Cep = unidadeDto.Cep,
                Logradouro = unidadeDto.Logradouro,
                Numero = unidadeDto.Numero,
                Bairro = unidadeDto.Bairro,
                Cidade = unidadeDto.Cidade,
                Estado = unidadeDto.Estado,
                TipoCozinha = unidadeDto.TipoCozinha,
                HorarioAbertura = unidadeDto.HorarioAbertura,
                HorarioFechamento = unidadeDto.HorarioFechamento,
                TaxaEntrega = unidadeDto.TaxaEntrega,
                Ativo = true
            };

            await _unidadeRepository.AdicionarAsync(novaUnidade);

            return MapearParaResponseDto(novaUnidade);
        }

        public async Task<bool> AtualizarUnidadeAsync(int id, UnidadeCreateDto unidadeDto)
        {
            if (unidadeDto == null) throw new ArgumentNullException(nameof(unidadeDto));

            var unidade = await _unidadeRepository.ObterPorIdAsync(id);

            if (unidade == null || !unidade.Ativo) return false;

            if (unidade.Cnpj != unidadeDto.Cnpj)
            {
                var cnpjExistente = await _unidadeRepository.ExisteCnpjAsync(unidadeDto.Cnpj);
                if (cnpjExistente)
                {
                    throw new InvalidOperationException("Não é possível alterar para este CNPJ, pois ele já pertence a outra unidade.");
                }
            }

            unidade.Cnpj = unidadeDto.Cnpj;
            unidade.Nome = unidadeDto.Nome;
            unidade.Telefone = unidadeDto.Telefone;
            unidade.Cep = unidadeDto.Cep;
            unidade.Logradouro = unidadeDto.Logradouro;
            unidade.Numero = unidadeDto.Numero;
            unidade.Bairro = unidadeDto.Bairro;
            unidade.Cidade = unidadeDto.Cidade;
            unidade.Estado = unidadeDto.Estado;
            unidade.TipoCozinha = unidadeDto.TipoCozinha;
            unidade.HorarioAbertura = unidadeDto.HorarioAbertura;
            unidade.HorarioFechamento = unidadeDto.HorarioFechamento;
            unidade.TaxaEntrega = unidadeDto.TaxaEntrega;

            await _unidadeRepository.AtualizarAsync(unidade);
            return true;
        }

        public async Task<bool> DesativarUnidadeAsync(int id)
        {
            var unidade = await _unidadeRepository.ObterPorIdAsync(id);
            if (unidade == null || !unidade.Ativo) return false;

            unidade.Ativo = false;

            await _unidadeRepository.AtualizarAsync(unidade);
            return true;
        }

        #region Mapeamentos Auxiliares

        private static UnidadeResponseDto MapearParaResponseDto(Unidade unidade)
        {
            return new UnidadeResponseDto
            {
                Id = unidade.Id,
                Nome = unidade.Nome,
                PossuiCozinhaCompleta = unidade.TipoCozinha == TipoCozinha.Completa,
                HorarioAbertura = unidade.HorarioAbertura.ToString("HH:mm"),
                HorarioFechamento = unidade.HorarioFechamento.ToString("HH:mm"),
                TaxaEntrega = unidade.TaxaEntrega,
                Cep = unidade.Cep,
                Logradouro = unidade.Logradouro,
                Numero = unidade.Numero,
                Bairro = unidade.Bairro,
                Cidade = unidade.Cidade,
                Estado = unidade.Estado
            };
        }

        #endregion
    }
}