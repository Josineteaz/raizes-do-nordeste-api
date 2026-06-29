using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Infrastructure.Data;


namespace RaizesDoNordeste.API.Application.Services
{
    public class CancelamentoPedidoWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CancelamentoPedidoWorker> _logger;

        public CancelamentoPedidoWorker(IServiceProvider serviceProvider, ILogger<CancelamentoPedidoWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CancelamentoPedidoWorker inicializado com sucesso.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Iniciando varredura de pedidos com reserva expirada...");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // Define a janela limite de tolerância: 10 minutos atrás
                        var tempoLimite = DateTime.UtcNow.AddMinutes(-10);

                        // Busca no banco todos os pedidos que ficaram presos no status AguardandoPagamento
                        // e que passaram do tempo limite de 10 minutos.
                        var pedidosExpirados = await context.Pedidos
                            .Where(p => p.Status == StatusPedido.AguardandoPagamento && p.DataPedido < tempoLimite)
                            .ToListAsync(stoppingToken);

                        if (pedidosExpirados.Any())
                        {

                            foreach (var pedido in pedidosExpirados)
                            {
                                // Altera o status para Cancelado.
                                pedido.Status = StatusPedido.Cancelado;

                                _logger.LogInformation($"Pedido ID {pedido.Id} (RN-{pedido.Id.ToString().PadLeft(5, '0')}) foi cancelado por inatividade.");
                            }

                            await context.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation("Pedidos expirados atualizados no banco com sucesso.");
                        }
                        else
                        {
                            _logger.LogInformation("Nenhum pedido expirado encontrado nesta rodada.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocorreu um erro ao processar a rotina de cancelamento de pedidos.");
                }

                // Aguarda 1 minuto antes de realizar a próxima verificação no banco de dados
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("CancelamentoPedidoWorker está sendo finalizado.");
        }
    }
}