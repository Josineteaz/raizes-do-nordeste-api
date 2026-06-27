namespace RaizesDoNordeste.API.Application.DTOs
{
    public class PedidoFilaBalcaoDto
    {
        public int PedidoId { get; set; }
        public int PedidoNumero { get; set; }
        public string CanalPedido { get; set; } = string.Empty;
        public DateTime DataHoraPronto { get; set; }
        public List<ItemFilaBalcaoDto> Itens { get; set; } = new();
    }

    public class ItemFilaBalcaoDto
    {
        public string ProdutoNome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }
}