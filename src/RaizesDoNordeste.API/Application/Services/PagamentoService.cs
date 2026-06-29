using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Enums;
using RaizesDoNordeste.API.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RaizesDoNordeste.API.Application.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IEstoqueMovimentoService _estoqueMovimentoService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuditoriaRepository _auditoriaRepository;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IUnitOfWork _uow;

        public PagamentoService(
            IPedidoRepository pedidoRepository,
            IUsuarioRepository usuarioRepository,
            IEstoqueMovimentoService estoqueMovimentoService,
            IAuditoriaRepository auditoriaRepository,
            IPagamentoRepository pagamentoRepository,
            IUnitOfWork uow)
        {
            _pedidoRepository = pedidoRepository;
            _usuarioRepository = usuarioRepository;
            _estoqueMovimentoService = estoqueMovimentoService;
            _auditoriaRepository = auditoriaRepository;
            _pagamentoRepository = pagamentoRepository;
            _uow = uow;
        }

        public async Task<PagamentoResponseDto> ProcessarPagamentoSimuladoAsync(PagamentoRequestDto request)
        {
            long pedidoId = request.PedidoId;

            var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);
            if (pedido == null)
            {
                return PagamentoResponseDto.Recusado(0, (int)pedidoId, "Pedido não encontrado no sistema.", "ERR-404");
            }

            if (pedido.Status != StatusPedido.AguardandoPagamento)
            {
                return PagamentoResponseDto.Recusado(0, (int)pedidoId, $"Este pedido não pode ser processado pois está com status: {pedido.Status}", "ERR-409");
            }

            await Task.Delay(1500);

            bool aprovado = request.Status == StatusPagamento.Aprovado;

            await _uow.BeginTransactionAsync();

            try
            {
                if (aprovado)
                {
                    if (pedido.Itens == null || !pedido.Itens.Any())
                    {
                        return PagamentoResponseDto.Recusado(0, (int)pedidoId, "Erro interno: Os itens do pedido não foram carregados.", "ERR-500");
                    }

                    foreach (var item in pedido.Itens)
                    {
                        bool baixouComSucesso = await _estoqueMovimentoService.DeduzirEstoqueAsync((int)item.ProdutoId, (int)pedido.UnidadeId, item.Quantidade);
                        if (!baixouComSucesso)
                        {
                            await _uow.RollbackAsync();
                            return PagamentoResponseDto.Recusado(0, (int)pedidoId, "Venda estornada por quebra de estoque concorrente.", "ERR-STK");
                        }
                    }

                    if (pedido.ClienteId.HasValue && pedido.ClienteId.Value > 0)
                    {
                        var cliente = await _usuarioRepository.ObterPorIdAsync(pedido.ClienteId.Value);
                        if (cliente != null)
                        {
                            if (pedido.ValorDesconto.HasValue && pedido.ValorDesconto.Value > 0)
                            {
                                int pontosGastos = (int)(pedido.ValorDesconto.Value * 10);
                                cliente.SaldoPontos = Math.Max(0, (cliente.SaldoPontos ?? 0) - pontosGastos);
                                cliente.FidelidadeMovimentos?.Add(new FidelidadeMovimento { ClienteId = cliente.Id, PedidoId = pedido.Id, Pontos = pontosGastos, TipoMovimento = TipoMovimentoFidelidade.Resgate });
                            }

                            int pontosGanhos = (int)(pedido.ValorPago - (pedido.TaxaEntrega ?? 0));
                            if (pontosGanhos > 0)
                            {
                                cliente.SaldoPontos = (cliente.SaldoPontos ?? 0) + pontosGanhos;
                                cliente.FidelidadeMovimentos?.Add(new FidelidadeMovimento { ClienteId = cliente.Id, PedidoId = pedido.Id, Pontos = pontosGanhos, TipoMovimento = TipoMovimentoFidelidade.Ganho });
                            }

                            await _usuarioRepository.AtualizarAsync(cliente);
                        }
                    }

                    pedido.Status = StatusPedido.Pago;
                    await _pedidoRepository.AtualizarAsync(pedido);

                    string transacaoId = $"TX-PED{pedidoId}-{new Random().Next(100000, 999999)}";
                    var novoPagamento = new Pagamento(pedidoId, pedido.ValorPago, "MOCK");
                    novoPagamento.RegistrarSucesso(transacaoId, $"{{\"status\": \"APPROVED\"}}");

                    await _pagamentoRepository.AdicionarAsync(novoPagamento);

                    await _uow.SaveChangesAsync();
                    await _uow.CommitAsync();

                    return PagamentoResponseDto.Aprovado((int)novoPagamento.Id, (int)pedidoId, transacaoId);
                }
                else
                {
                    string transacaoRecusadaId = $"TX-REF{pedidoId}-{new Random().Next(100000, 999999)}";
                    var pagamentoFalho = new Pagamento(pedidoId, pedido.ValorPago, "MOCK_RECUSADO");
                    pagamentoFalho.RegistrarRecusa(transacaoRecusadaId, "{\"status\": \"DECLINED\"}");

                    await _pagamentoRepository.AdicionarAsync(pagamentoFalho);

                    var logAud = new Auditoria { Acao = "ACESSO_SENSIVEL", Entidade = "Pagamento", RegistroId = pedido.Id, DataAcao = DateTime.UtcNow, Descricao = "Pagamento recusado", UsuarioId = pedido.ClienteId, UnidadeId = pedido.UnidadeId };
                    await _auditoriaRepository.SalvarAuditoriaAsync(logAud);

                    await _uow.SaveChangesAsync();
                    await _uow.CommitAsync();

                    return PagamentoResponseDto.Recusado((int)pagamentoFalho.Id, (int)pedidoId, "Mock recusado.", transacaoRecusadaId);
                }
            }
            catch (Exception)
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}