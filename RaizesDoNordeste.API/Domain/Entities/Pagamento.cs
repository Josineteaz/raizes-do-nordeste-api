using RaizesDoNordeste.API.Domain.Enums;
namespace RaizesDoNordeste.API.Domain.Entities
{

    public class Pagamento
    {
        public long Id { get; private set; }
        public long PedidoId { get; private set; }

        public Pedido Pedido { get; private set; }

        public decimal Valor { get; private set; }
        public string FormaPagamento { get; private set; }
        public StatusPagamento Status { get; private set; }

        public string GatewayTransacaoId { get; private set; }

        public string GatewayPayloadRetorno { get; private set; }

        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }

        protected Pagamento() { }

        public Pagamento(long pedidoId, decimal valor, string formaPagamento)
        {
            PedidoId = pedidoId;
            Valor = valor;
            FormaPagamento = formaPagamento;
            Status = StatusPagamento.Pendente;
            DataCriacao = DateTime.UtcNow;
        }

        public void RegistrarSucesso(string transacaoId, string payload)
        {
            Status = StatusPagamento.Aprovado;
            GatewayTransacaoId = transacaoId;
            GatewayPayloadRetorno = payload;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void RegistrarRecusa(string transacaoId, string payload)
        {
            Status = StatusPagamento.Recusado;
            GatewayTransacaoId = transacaoId;
            GatewayPayloadRetorno = payload;
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}