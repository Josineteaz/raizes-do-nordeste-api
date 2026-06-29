using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Enums;
using RaizesDoNordeste.API.Domain.Interfaces;


namespace RaizesDoNordeste.API.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IEstoqueMovimentoService _estoqueMovimentoService;
        private readonly IFidelidadeService _fidelidadeService;
        private readonly IPromocaoRepository _promocaoRepository;
        private readonly IUserContext _userContext;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IUsuarioRepository usuarioRepository,
            IProdutoRepository produtoRepository,
            IEstoqueMovimentoService estoqueMovimentoService,
            IFidelidadeService fidelidadeService,
            IUserContext userContext,
            IPromocaoRepository promocaoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _usuarioRepository = usuarioRepository;
            _produtoRepository = produtoRepository;
            _estoqueMovimentoService = estoqueMovimentoService;
            _fidelidadeService = fidelidadeService;
            _promocaoRepository = promocaoRepository;
            _userContext = userContext;
        }

        public async Task<PedidoResultadoDto> CriarPedidoAsync(PedidoCreateDto pedidoDto)
        {
            if (_userContext.IsInRole("Cliente"))
            {
                var usuarioLogadoId = _userContext.ObterUsuarioLogadoId();
                if (pedidoDto.ClienteId.HasValue && pedidoDto.ClienteId != usuarioLogadoId)
                {
                    return PedidoResultadoDto.Falha("Forbidden", "Você não tem permissão para criar um pedido para outro cliente.");
                }
            }
            else
            {
                if (!(_userContext.IsInRole("Cliente") || _userContext.IsInRole("Atendente") || _userContext.IsInRole("Atendente")))
                {
                    return PedidoResultadoDto.Falha("Forbidden", "Perfil de usuário não possui permissão para gerar pedidos.");
                }
            }

            // Validação do Canal de Pedido
            if (!Enum.IsDefined(typeof(CanalPedido), pedidoDto.CanalPedido))
            {
                return PedidoResultadoDto.Falha("InvalidChannel", "O canal de origem do pedido informado é inválido.");
            }

            // Valida se o cliente existe no sistema
            Usuario? cliente = null;
            if (pedidoDto.ClienteId.HasValue && pedidoDto.ClienteId.Value > 0)
            {
                cliente = await _usuarioRepository.ObterPorIdAsync(pedidoDto.ClienteId.Value);
                if (cliente == null)
                    return PedidoResultadoDto.Falha("NotFound", "Cliente não encontrado.");

                // Bloqueia novas compras se o consentimento foi revogado
                if (!cliente.ConsentimentoLgpd)
                {
                    return PedidoResultadoDto.Falha("LgpdConsentRequired", "Não é possível processar novas compras para um usuário com o consentimento LGPD revogado.");
                }
            }
            
            // Busca se o cliente já possui QUALQUER pedido pendente de pagamento
            if (pedidoDto.ClienteId.HasValue && pedidoDto.ClienteId.Value > 0)
            {
                var possuiPedidoPendente = await _pedidoRepository.ExistePedidoPendentePorClienteAsync(pedidoDto.ClienteId.Value);
                if (possuiPedidoPendente)
                {
                    return PedidoResultadoDto.Falha("PendingOrderBlock", "Você já possui um pedido aguardando pagamento. Finalize ou cancele o pedido anterior antes de efetuar uma nova compra.");
                }
            }

            decimal valorTotalBrutoDoPedido = 0;
            var itensValidados = new List<(Produto Produto, int Quantidade, decimal PrecoPraticado)>();
            var dataAtual = DateTime.UtcNow;

            // Busca todas as promoções ativas para a unidade atual
            var promocoesDaUnidade = await _promocaoRepository.ObterTodasAsync();
            var promocoesAtivas = promocoesDaUnidade
                .Where(p => p.UnidadeId == pedidoDto.UnidadeId && dataAtual >= p.DataInicio && dataAtual <= p.DataFim)
                .ToList();

            foreach (var item in pedidoDto.Itens)
            {
                var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                if (produto == null)
                {
                    return PedidoResultadoDto.Falha("ProductNotFound", $"Produto com ID {item.ProdutoId} não existe no catálogo.");
                }

                // SAZONALIDADE: BLOQUEIO FORA DE ÉPOCA 
                if (produto.ProdutoSazonalidades != null && produto.ProdutoSazonalidades.Count > 0)
                {
                    int mesAtual = DateTime.UtcNow.Month;
                    bool permiteNoMesAtual = produto.ProdutoSazonalidades.Any(s => s.Mes == mesAtual);

                    if (!permiteNoMesAtual)
                    {
                        return PedidoResultadoDto.Falha("SeasonalBlock", $"O produto '{produto.Nome}' é sazonal e não está disponível para venda no mês atual (Mês {mesAtual}).");
                    }
                }

                // VALIDAÇÃO DE ESTOQUE
                var temEstoque = await _estoqueMovimentoService.TemEstoqueSuficienteAsync(item.ProdutoId, pedidoDto.UnidadeId, item.Quantidade);
                if (!temEstoque)
                {
                    return PedidoResultadoDto.Falha("InsufficientStock", $"Produto '{produto.Nome}' (ID {item.ProdutoId}) não possui estoque disponível suficiente nesta unidade.");
                }

                // CHECAGEM DE PROMOÇÃO ATIVA DO PRODUTO
                decimal precoOriginalDoBanco = produto.Preco;
                decimal precoFinalDoItem = precoOriginalDoBanco;

                var promocaoDoProduto = promocoesAtivas.FirstOrDefault(p =>
                    p.ProdutoId == produto.Id &&
                    p.UnidadeId == pedidoDto.UnidadeId);

                if (promocaoDoProduto != null)
                {
                    if (promocaoDoProduto.TipoDesconto == TipoDesconto.Percentual)
                    {
                        precoFinalDoItem = precoOriginalDoBanco - (precoOriginalDoBanco * (promocaoDoProduto.ValorDesconto / 100));
                    }
                    else if (promocaoDoProduto.TipoDesconto == TipoDesconto.ValorFixo)
                    {
                        precoFinalDoItem = precoOriginalDoBanco - promocaoDoProduto.ValorDesconto;
                    }

                    if (precoFinalDoItem <= 0)
                    {
                        precoFinalDoItem = 0.01m;
                    }
                }
                else
                {
                    precoFinalDoItem = precoOriginalDoBanco;
                }

                valorTotalBrutoDoPedido += precoFinalDoItem * item.Quantidade;
                itensValidados.Add((produto, item.Quantidade, precoFinalDoItem));
            }

            // PROCESSAMENTO E VALIDAÇÃO DO DESCONTO DE FIDELIDADE
            decimal valorDescontoFidelidade = 0;

            if (pedidoDto.PontosParaUtilizar.HasValue && pedidoDto.PontosParaUtilizar.Value > 0)
            {
                var canalAtual = (CanalPedido)pedidoDto.CanalPedido;

                if (canalAtual == CanalPedido.Totem || canalAtual == CanalPedido.Balcao)
                {
                    return PedidoResultadoDto.Falha("InvalidChannelForReward", "O resgate de pontos só é permitido em canais autoatendidos logados (App ou Web).");
                }

                if (pedidoDto.PontosParaUtilizar.Value % 50 != 0)
                {
                    return PedidoResultadoDto.Falha("InvalidPoints", "Os pontos para desconto devem ser múltiplos de 50.");
                }
                if (!pedidoDto.ClienteId.HasValue || pedidoDto.ClienteId.Value <= 0)
                {
                    return PedidoResultadoDto.Falha("AnonymousFidelityBlock", "Não é possível utilizar pontos de fidelidade em um pedido sem cliente identificado.");
                }

                int saldoDisponivel = await _fidelidadeService.ObterPontosPorClienteAsync(pedidoDto.ClienteId.Value);
                if (saldoDisponivel < pedidoDto.PontosParaUtilizar.Value)
                {
                    return PedidoResultadoDto.Falha("InsufficientPoints", $"Saldo de pontos insuficiente. Você possui {saldoDisponivel} pontos.");
                }

                valorDescontoFidelidade = (pedidoDto.PontosParaUtilizar.Value / 50) * 5;
            }

            // CÁLCULO DA TAXA DE ENTREGA
            decimal taxaEntregaAplicada = 0;
            var modalidadeEnum = (ModalidadePedido)pedidoDto.Modalidade;

            if (modalidadeEnum == ModalidadePedido.Delivery)
            {
                taxaEntregaAplicada = 7.50m;
            }

            decimal valorProdutosComDesconto = Math.Max(0, valorTotalBrutoDoPedido - valorDescontoFidelidade);
            decimal valorFinalAPagar = valorProdutosComDesconto + taxaEntregaAplicada;

            // PERSISTE O PEDIDO
            int ultimoNumero = await _pedidoRepository.ObterUltimoNumeroPedidoPorUnidadeAsync(pedidoDto.UnidadeId);
            int proximoNumeroUnidade = ultimoNumero + 1;
            var pedidoEntidade = new Pedido
            {
                ClienteId = pedidoDto.ClienteId,
                UnidadeId = pedidoDto.UnidadeId,
                CanalPedido = (CanalPedido)pedidoDto.CanalPedido,
                NumPedidoUnidade = proximoNumeroUnidade,
                Modalidade = modalidadeEnum,
                Observacao = pedidoDto.Observacao,
                TaxaEntrega = taxaEntregaAplicada > 0 ? taxaEntregaAplicada : null,
                ValorDesconto = valorDescontoFidelidade > 0 ? valorDescontoFidelidade : null,
                ValorPago = valorFinalAPagar,
                Status = StatusPedido.AguardandoPagamento,
                DataPedido = DateTime.UtcNow,
                Itens = new List<PedidoItem>()
            };

            foreach (var validItem in itensValidados)
            {
                pedidoEntidade.Itens.Add(new PedidoItem
                {
                    ProdutoId = validItem.Produto.Id,
                    Quantidade = validItem.Quantidade,
                    PrecoUnitario = validItem.PrecoPraticado
                });
            }

            await _pedidoRepository.AdicionarAsync(pedidoEntidade);

            // Mapeia e constrói o DTO de resposta final
            string chavePagamento = $"TX-PED{pedidoEntidade.Id}-{new Random().Next(100000, 999999)}";
            var responseData = new PedidoResponseDto
            {
                PedidoId = pedidoEntidade.Id,
                PedidoNumero = pedidoEntidade.NumPedidoUnidade,
                ClienteId = pedidoEntidade.ClienteId,
                UnidadeId = pedidoEntidade.UnidadeId,
                Status = pedidoEntidade.Status.ToString(),
                CanalPedido = pedidoEntidade.CanalPedido.ToString(),
                PontosFidelidadeGanhos = 0,
                ChavePagamento = chavePagamento,
                CriadoEm = pedidoEntidade.DataPedido,
                ValorBrutoProdutos = valorTotalBrutoDoPedido,
                TaxaEntrega = taxaEntregaAplicada > 0 ? taxaEntregaAplicada : null,
                ValorDesconto = valorDescontoFidelidade > 0 ? valorDescontoFidelidade : null,
                ValorTotal = valorFinalAPagar,

                Itens = pedidoEntidade.Itens.Select(itemEntity =>
                {
                    var validado = itensValidados.First(x => x.Produto.Id == itemEntity.ProdutoId);
                    return new ItemPedidoResponseDto
                    {
                        ProdutoId = itemEntity.ProdutoId,
                        ProdutoNome = validado.Produto.Nome,
                        Quantidade = itemEntity.Quantidade,
                        PrecoUnitario = itemEntity.PrecoUnitario
                    };
                }).ToList()
            };


            return PedidoResultadoDto.SucessoResultado(pedidoEntidade.Id, responseData);
        }

        public async Task<PedidoResponseDto?> ObterPedidoPorIdAsync(long id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null) return null;

            return new PedidoResponseDto
            {
                PedidoId = pedido.Id,
                PedidoNumero = pedido.NumPedidoUnidade,
                ClienteId = pedido.ClienteId,
                UnidadeId = pedido.UnidadeId,
                ValorTotal = pedido.ValorPago,
                Status = pedido.Status.ToString(),
                CanalPedido = pedido.CanalPedido.ToString(),
                CriadoEm = pedido.DataPedido
            };
        }


        public async Task<IEnumerable<PedidoResponseDto>> ObterPedidosAsync(int? unidadeId, string? canalPedido, int page = 1, int limit = 10)
        {
            var pedidosPaginados = await _pedidoRepository.ObterTodosPaginadosAsync(unidadeId, canalPedido, page, limit);

            return pedidosPaginados.Select(pedido => new PedidoResponseDto
            {
                PedidoId = pedido.Id,
                PedidoNumero = pedido.NumPedidoUnidade,
                ClienteId = pedido.ClienteId,
                UnidadeId = pedido.UnidadeId,
                ValorTotal = pedido.ValorPago,
                Status = pedido.Status.ToString(),
                CanalPedido = pedido.CanalPedido.ToString(),
                CriadoEm = pedido.DataPedido
            }).ToList();
        }
        public async Task<bool> AtualizarStatusAsync(long id, StatusPedido novoStatus)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null) return false;

            if (pedido.Status == novoStatus)
            {
                throw new InvalidOperationException($"O pedido já se encontra no status '{pedido.Status}'. Nenhuma transição foi realizada.");
            }

            if (!_userContext.IsInRole("Atendente") && novoStatus == StatusPedido.Entregue)
            {
                throw new InvalidOperationException($"Transição inválida. Só Atendente pode mudar o status para Entregue.");
            }

            if (pedido.Status == StatusPedido.AguardandoPagamento &&
                novoStatus != StatusPedido.Pago && novoStatus != StatusPedido.Cancelado)
            {
                throw new InvalidOperationException($"Transição inválida. Pedido aguardando pagamento não pode ir direto para {novoStatus}.");
            }

            if (pedido.Status == StatusPedido.Pago &&
                novoStatus != StatusPedido.EmPreparo && novoStatus != StatusPedido.Cancelado)
            {
                throw new InvalidOperationException($"Transição inválida. Pedido pago deve entrar em preparação antes de ir para {novoStatus}.");
            }

            if (pedido.Status == StatusPedido.EmPreparo &&
                novoStatus != StatusPedido.Pronto && novoStatus != StatusPedido.Cancelado)
            {
                throw new InvalidOperationException($"Transição inválida. Pedido em preparo deve ser marcado como Pronto primeiro.");
            }

            if (pedido.Status == StatusPedido.Entregue || pedido.Status == StatusPedido.Cancelado)
            {
                throw new InvalidOperationException("Não é permitido alterar o status de um pedido já Finalizado ou Cancelado.");
            }

            pedido.Status = novoStatus;
            await _pedidoRepository.AtualizarAsync(pedido);

            return true;
        }

        public async Task<IEnumerable<PedidoFilaProducaoDto>> ObterPedidosPagosPorUnidadeAsync(int unidadeId, int page = 1, int limit = 10)
        {
            var todosPedidos = await _pedidoRepository.ObterTodosAsync();

            // Filtra por Unidade, Status igual a Pago e ordena cronologicamente (mais antigo primeiro)
            var pedidosFiltrados = todosPedidos
                .Where(p => p.UnidadeId == unidadeId && p.Status == StatusPedido.Pago)
                .OrderBy(p => p.DataPedido);

            var pedidosPaginados = pedidosFiltrados
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            // Mapeia os dados para o DTO otimizado da cozinha
            return pedidosPaginados.Select(p => new PedidoFilaProducaoDto
            {
                PedidoId = (int)p.Id,
                Observacoes = p.Observacao ?? string.Empty,
                DataHoraCriacao = p.DataPedido,
                Itens = p.Itens?.Select(i => new ItemFilaProducaoDto
                {
                    ProdutoNome = i.Produto?.Nome ?? $"Produto ID {i.ProdutoId}",
                    Quantidade = i.Quantidade
                }).ToList() ?? new List<ItemFilaProducaoDto>()
            });
        }

        public async Task<IEnumerable<PedidoFilaBalcaoDto>> ObterPedidosProntosPorUnidadeAsync(int unidadeId, int page = 1, int limit = 10)
        {
            var todosPedidos = await _pedidoRepository.ObterTodosAsync();

            // Filtra pela unidade informada e pelo Status 4 (Pronto)
            var pedidosProntos = todosPedidos
                .Where(p => p.UnidadeId == unidadeId && p.Status == StatusPedido.Pronto)
                .OrderBy(p => p.DataPedido); // Ordena pelo mais antigo que ficou pronto

            var pedidosPaginados = pedidosProntos
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            return pedidosPaginados.Select(p => new PedidoFilaBalcaoDto
            {
                PedidoId = (int)p.Id,
                PedidoNumero = p.NumPedidoUnidade,
                CanalPedido = p.CanalPedido.ToString(),
                DataHoraPronto = p.DataPedido,
                Itens = p.Itens?.Select(i => new ItemFilaBalcaoDto
                {
                    ProdutoNome = i.Produto?.Nome ?? $"Produto ID {i.ProdutoId}",
                    Quantidade = i.Quantidade
                }).ToList() ?? new List<ItemFilaBalcaoDto>()
            });
        }
    }
}