using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Domain.Entities
{
    public class FidelidadeMovimento
    {
        public long Id { get; set; }
        public int ClienteId { get; set; }
        public long PedidoId { get; set; }
        public TipoMovimentoFidelidade TipoMovimento { get; set; }
        public int Pontos { get; set; }

        public Usuario Cliente { get; set; } = null!;
        public Pedido Pedido { get; set; } = null!;
    }
}
