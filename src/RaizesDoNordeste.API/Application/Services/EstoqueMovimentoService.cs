using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Enums;
using RaizesDoNordeste.API.Domain.Interfaces;


namespace RaizesDoNordeste.API.Application.Services
{
    public class EstoqueMovimentoService : IEstoqueMovimentoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IInsumoRepository _insumoRepository;
        private readonly IProdutoFichaTecnicaRepository _fichaTecnicaRepository;
        private readonly IEstoqueMovimentoRepository _estoqueMovimentoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EstoqueMovimentoService(
            IProdutoRepository produtoRepository,
            IInsumoRepository insumoRepository,
            IProdutoFichaTecnicaRepository fichaTecnicaRepository,
            IEstoqueMovimentoRepository estoqueMovimentoRepository,
            IPedidoRepository pedidoRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _produtoRepository = produtoRepository;
            _insumoRepository = insumoRepository;
            _fichaTecnicaRepository = fichaTecnicaRepository;
            _estoqueMovimentoRepository = estoqueMovimentoRepository;
            _pedidoRepository = pedidoRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> ObterSaldoAtualAsync(int produtoId, int unidadeId)
        {
            ValidarEscopoUnidade(unidadeId);

            var produto = await _produtoRepository.ObterPorIdAsync(produtoId);

            if (produto == null)
                throw new InvalidOperationException("Produto não encontrado.");

            if (!produto.Ativo)
                throw new InvalidOperationException("O produto consultado está inativo.");

            var fichasTecnicas = await _fichaTecnicaRepository.ObterPorProdutoIdAsync(produtoId);

            if (fichasTecnicas == null || !fichasTecnicas.Any())
                throw new InvalidOperationException("Este produto não possui uma ficha técnica (receita) configurada.");

            var limitesPossiveis = new List<int>();

            foreach (var ficha in fichasTecnicas)
            {
                var insumo = await _insumoRepository.ObterPorIdAsync(ficha.InsumoId);

                if (insumo == null || insumo.UnidadeId != unidadeId) return 0;
                if (ficha.Quantidade == 0) continue;

                int capacidadePorInsumo = (int)(insumo.QuantidadeAtual / ficha.Quantidade);
                limitesPossiveis.Add(capacidadePorInsumo);
            }

            return limitesPossiveis.Any() ? limitesPossiveis.Min() : 0;
        }

        public async Task<bool> RegistrarMovimentacaoAsync(EstoqueMovimentoDto movimentoDto)
        {
            if (movimentoDto == null)
                throw new ArgumentNullException(nameof(movimentoDto));

            ValidarEscopoUnidade(movimentoDto.UnidadeId);

            var insumo = await _insumoRepository.ObterPorIdAsync(movimentoDto.InsumoId);

            if (insumo == null || insumo.UnidadeId != movimentoDto.UnidadeId)
                throw new InvalidOperationException("Insumo não encontrado ou não pertence a esta unidade.");

            TipoMovimentoEstoque tipoEnum = movimentoDto.TipoMovimento;
            if (tipoEnum == TipoMovimentoEstoque.Entrada)
            {
                insumo.QuantidadeAtual += movimentoDto.Quantidade;
            }
            else
            {
                if (insumo.QuantidadeAtual < movimentoDto.Quantidade)
                {
                    throw new InvalidOperationException($"Saldo insuficiente para saída. Saldo disponível de '{insumo.Nome}': {insumo.QuantidadeAtual}.");
                }
                insumo.QuantidadeAtual -= movimentoDto.Quantidade;
            }

            var novoMovimento = new EstoqueMovimento
            {
                UnidadeId = movimentoDto.UnidadeId,
                InsumoId = insumo.Id,
                TipoMovimento = tipoEnum,
                Quantidade = movimentoDto.Quantidade,
                DataMovimento = DateTime.UtcNow,
                Observacao = $"Movimentação manual via painel administrativo"
            };

            await _insumoRepository.AtualizarAsync(insumo);
            await _estoqueMovimentoRepository.AdicionarAsync(novoMovimento);

            return true;
        }

        public async Task<bool> TemEstoqueSuficienteAsync(int produtoId, int unidadeId, int quantidadePedido)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(produtoId);
            if (produto == null || !produto.Ativo) return false;

            var fichasTecnicas = await _fichaTecnicaRepository.ObterPorProdutoIdAsync(produtoId);
            if (fichasTecnicas == null || !fichasTecnicas.Any()) return false;

            var dezMinutosAtras = DateTime.UtcNow.AddMinutes(-10);

            foreach (var ficha in fichasTecnicas)
            {
                var insumo = await _insumoRepository.ObterPorIdAsync(ficha.InsumoId);
                if (insumo == null || insumo.UnidadeId != unidadeId) return false;

                decimal totalReservadoAtivo = await _pedidoRepository.ObterTotalReservadoAsync(produtoId, unidadeId, dezMinutosAtras, ficha.Quantidade);

                decimal quantidadeNecessaria = ficha.Quantidade * quantidadePedido;
                decimal estoqueDisponivel = insumo.QuantidadeAtual - totalReservadoAtivo;

                if (estoqueDisponivel < quantidadeNecessaria)
                    return false;
            }

            return true;
        }

        public async Task<bool> DeduzirEstoqueAsync(int produtoId, int unidadeId, int quantidadePedido)
        {
            var temSaldo = await TemEstoqueSuficienteAsync(produtoId, unidadeId, quantidadePedido);
            if (!temSaldo) return false;

            var produto = await _produtoRepository.ObterPorIdAsync(produtoId);
            if (produto == null) return false;

            var fichasTecnicas = await _fichaTecnicaRepository.ObterPorProdutoIdAsync(produtoId);

            foreach (var ficha in fichasTecnicas)
            {
                var insumo = await _insumoRepository.ObterPorIdAsync(ficha.InsumoId);
                if (insumo == null || insumo.UnidadeId != unidadeId) continue;

                decimal quantidadeNecessaria = ficha.Quantidade * quantidadePedido;

                insumo.QuantidadeAtual -= quantidadeNecessaria;
                await _insumoRepository.AtualizarAsync(insumo);

                var movimentoSaida = new EstoqueMovimento
                {
                    UnidadeId = unidadeId,
                    InsumoId = insumo.Id,
                    TipoMovimento = TipoMovimentoEstoque.Saída,
                    Quantidade = quantidadeNecessaria,
                    DataMovimento = DateTime.UtcNow,
                    Observacao = $"Baixa automática - Venda de {quantidadePedido}x {produto.Nome} (ID {produtoId})"
                };

                await _estoqueMovimentoRepository.AdicionarAsync(movimentoSaida);
            }

            return true;
        }

        #region Mecanismo de Segurança Centralizado

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
                        throw new UnauthorizedAccessException("Acesso Negado. Você não possui autorização para gerenciar ou consultar o estoque desta filial.");
                    }
                    return;
                }
            }

            throw new UnauthorizedAccessException("Acesso Negado. Credenciais inválidas para operação de escopo de estoque.");
        }

        #endregion
    }
}