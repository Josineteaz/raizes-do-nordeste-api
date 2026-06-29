using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProdutoService(
            IProdutoRepository produtoRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _produtoRepository = produtoRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProdutoResponseDto?> ObterPorIdAsync(int id)
        {
            var produto = await _produtoRepository.ObterPorIdComSazonalidadesAsync(id);
            if (produto == null || !produto.Ativo) return null;

            return MapearParaResponseDto(produto);
        }


        public async Task<IEnumerable<ProdutoResponseDto>> ObterTodosAsync(int unidadeId, int page = 1, int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1 || limit > 50) limit = 10;

            var produtos = await _produtoRepository.ObterTodosAsync(unidadeId, page, limit);

            return produtos
                .Select(MapearParaResponseDto)
                .ToList();
        }

        public async Task<IEnumerable<ProdutoResponseDto>> ObterPorCategoriaAsync(int unidadeId, int categoriaId, int page = 1, int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1 || limit > 50) limit = 10;

            var produtos = await _produtoRepository.ObterPorCategoriaAsync(unidadeId, categoriaId, page, limit);

            return produtos
                .Select(MapearParaResponseDto)
                .ToList();
        }
        public async Task<ProdutoResponseDto> CriarProdutoAsync(ProdutoCreateDto produtoDto)
        {
            if (produtoDto == null)
                throw new ArgumentNullException(nameof(produtoDto));

            ValidarEscopoUnidade(produtoDto.UnidadeId);

            var produtoExistente = await _produtoRepository.ObterPorNomeEUnidadeAsync(produtoDto.Nome.Trim(), produtoDto.UnidadeId);

            if (produtoExistente != null && produtoExistente.Ativo)
            {
                throw new InvalidOperationException($"Já existe um produto ativo cadastrado com o nome '{produtoDto.Nome}' para esta unidade.");
            }

            var novoProduto = new Produto
            {
                Nome = produtoDto.Nome.Trim(),
                Preco = produtoDto.Preco,
                CategoriaId = produtoDto.CategoriaId,
                UnidadeId = produtoDto.UnidadeId,
                UrlImagem = produtoDto.UrlImagem,
                Ativo = true
            };

            if (produtoDto.ESazonal && produtoDto.MesesSazonais != null && produtoDto.MesesSazonais.Any())
            {
                string descricaoDefinida = !string.IsNullOrWhiteSpace(produtoDto.DescricaoSazonal)
                    ? produtoDto.DescricaoSazonal
                    : "Produto integrado ao cardápio sazonal.";

                foreach (var mes in produtoDto.MesesSazonais)
                {
                    if (mes >= 1 && mes <= 12)
                    {
                        novoProduto.ProdutoSazonalidades.Add(new ProdutoSazonalidade
                        {
                            Mes = mes,
                            Descricao = descricaoDefinida
                        });
                    }
                }
            }

            await _produtoRepository.AdicionarAsync(novoProduto);
            return MapearParaResponseDto(novoProduto);
        }

        public async Task<bool> AtualizarProdutoAsync(int id, ProdutoCreateDto produtoDto)
        {
            if (produtoDto == null)
                throw new ArgumentNullException(nameof(produtoDto));

            var produto = await _produtoRepository.ObterPorIdComSazonalidadesAsync(id);
            if (produto == null || !produto.Ativo) return false;

            ValidarEscopoUnidade(produto.UnidadeId);
            ValidarEscopoUnidade(produtoDto.UnidadeId);

            produto.Nome = produtoDto.Nome.Trim();
            produto.Preco = produtoDto.Preco;
            produto.CategoriaId = produtoDto.CategoriaId;
            produto.UnidadeId = produtoDto.UnidadeId;
            produto.UrlImagem = produtoDto.UrlImagem;

            produto.ProdutoSazonalidades.Clear();

            if (produtoDto.ESazonal && produtoDto.MesesSazonais != null && produtoDto.MesesSazonais.Any())
            {
                string descricaoDefinida = !string.IsNullOrWhiteSpace(produtoDto.DescricaoSazonal)
                    ? produtoDto.DescricaoSazonal
                    : "Produto integrado ao cardápio sazonal.";

                foreach (var mes in produtoDto.MesesSazonais)
                {
                    if (mes >= 1 && mes <= 12)
                    {
                        produto.ProdutoSazonalidades.Add(new ProdutoSazonalidade
                        {
                            Mes = mes,
                            Descricao = descricaoDefinida
                        });
                    }
                }
            }

            await _produtoRepository.AtualizarAsync(produto);
            return true;
        }

        public async Task<bool> RemoverProdutoDaUnidadeAsync(int produtoId, int unidadId)
        {
            ValidarEscopoUnidade(unidadId);

            var produto = await _produtoRepository.ObterPorIdAsync(produtoId);
            if (produto == null || !produto.Ativo || produto.UnidadeId != unidadId)
            {
                return false;
            }

            produto.Ativo = false;
            await _produtoRepository.AtualizarAsync(produto);
            return true;
        }

        #region Métodos Privados de Suporte (Segurança e Mapeamento)

        private void ValidarEscopoUnidade(int unidadeIdAlvo)
        {
            var usuarioLogado = _httpContextAccessor.HttpContext?.User;
            if (usuarioLogado == null) return;

            if (usuarioLogado.IsInRole("AdministradorFranquia")) return;

            if (usuarioLogado.IsInRole("GerenteUnidade"))
            {
                var unidadeClaim = usuarioLogado.FindFirst("UnidadeId")?.Value;
                if (int.TryParse(unidadeClaim, out int unidadeGerenteId))
                {
                    if (unidadeGerenteId != unidadeIdAlvo)
                    {
                        throw new UnauthorizedAccessException("Acesso Negado. Você não possui privilégios para gerenciar produtos nesta filial.");
                    }
                    return;
                }
            }

            throw new UnauthorizedAccessException("Acesso Negado. Perfil de usuário inválido para validação de escopo.");
        }

        private static ProdutoResponseDto MapearParaResponseDto(Produto produto)
        {
            return new ProdutoResponseDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Preco = produto.Preco,
                UrlImagem = produto.UrlImagem,
                CategoriaId = produto.CategoriaId,
                UnidadeId = produto.UnidadeId,
                Ativo = produto.Ativo,
                IsSazonal = produto.ProdutoSazonalidades != null && produto.ProdutoSazonalidades.Any()
            };
        }

        #endregion
    }
}