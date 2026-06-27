using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class PagamentoResponseDto
    {
        public int PagamentoId { get; set; }
        public int PedidoId { get; set; }
        public StatusPagamento Status { get; set; }
        public string TransacaoId { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataProcessamento { get; set; }

        public bool Sucesso => Status == StatusPagamento.Aprovado;

        public static PagamentoResponseDto Aprovado(int pagamentoId, int pedidoId, string transacaoId) => new()
        {
            PagamentoId = pagamentoId,
            PedidoId = pedidoId,
            Status = StatusPagamento.Aprovado,
            TransacaoId = transacaoId,
            Mensagem = "Pagamento aprovado com sucesso.",
            DataProcessamento = DateTime.UtcNow
        };

        public static PagamentoResponseDto Recusado(int pagamentoId, int pedidoId, string motivo, string transacaoId) => new()
        {
            PagamentoId = pagamentoId,
            PedidoId = pedidoId,
            Status = StatusPagamento.Recusado,
            TransacaoId = transacaoId,
            Mensagem = motivo,
            DataProcessamento = DateTime.UtcNow
        };
    }
}