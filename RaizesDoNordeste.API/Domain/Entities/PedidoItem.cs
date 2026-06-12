using System;

namespace RaizesDoNordeste.API.Domain.Entities
{
    public class PedidoItem
    {
        public long Id { get; set; }
        public long PedidoId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }


        public Pedido Pedido { get; set; } = null!;
        public Produto Produto { get; set; } = null!;
    }
}