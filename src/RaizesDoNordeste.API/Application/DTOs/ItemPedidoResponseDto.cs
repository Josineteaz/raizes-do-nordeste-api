namespace RaizesDoNordeste.API.Application.DTOs
{
    public class ItemPedidoResponseDto
    {
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }

        public decimal PrecoTotal => Quantidade * PrecoUnitario;
    }
}