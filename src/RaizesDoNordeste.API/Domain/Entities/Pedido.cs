using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Domain.Entities
{
    public class Pedido
    {
        public long Id { get; set; }
        public int UnidadeId { get; set; }
        public int? ClienteId { get; set; } 
        public int NumPedidoUnidade { get; set; }
        public CanalPedido CanalPedido { get; set; }
        public ModalidadePedido? Modalidade { get; set; }
        public DateTime DataPedido { get; set; }
        public StatusPedido Status { get; set; }
        public decimal? TaxaEntrega { get; set; }
        public decimal? ValorDesconto { get; set; }
        public decimal ValorPago { get; set; }  
        public string? Observacao { get; set; }

        public Unidade Unidade { get; set; } = null!;
        public Usuario Cliente { get; set; } = null!;
        public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
        public ICollection<FidelidadeMovimento> FidelidadeMovimentos { get; set; } = new List<FidelidadeMovimento>();
        public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
    }
}