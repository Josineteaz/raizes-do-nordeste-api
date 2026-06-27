
namespace RaizesDoNordeste.API.Application.DTOs
{
    public class PedidoResponseDto
    {
        public long PedidoId { get; set; }
        public long PedidoNumero { get; set; }

        public int? ClienteId { get; set; }

        public string Status { get; set; } = string.Empty;
        public string CanalPedido { get; set; } = string.Empty;
        public int UnidadeId { get; set; }
        public int PontosFidelidadeGanhos { get; set; }
        public DateTime CriadoEm { get; set; }
        public List<ItemPedidoResponseDto> Itens { get; set; } = new();

        public decimal ValorBrutoProdutos { get; set; }

        public decimal? TaxaEntrega { get; set; }

        public decimal? ValorDesconto { get; set; }

        public decimal ValorTotal { get; set; }
        public string ChavePagamento { get; set; }
    }
}