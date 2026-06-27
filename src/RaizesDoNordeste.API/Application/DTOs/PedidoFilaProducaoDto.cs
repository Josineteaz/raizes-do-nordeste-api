namespace RaizesDoNordeste.API.Application.DTOs
{
    public class PedidoFilaProducaoDto
    {
        public int PedidoId { get; set; }
        public string Observacoes { get; set; } = string.Empty;
        public DateTime DataHoraCriacao { get; set; }
        public List<ItemFilaProducaoDto> Itens { get; set; } = new();
    }

    public class ItemFilaProducaoDto
    {
        public string ProdutoNome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }
}