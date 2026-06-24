namespace RaizesDoNordeste.API.Domain.Entities
{
    public class ProdutoSazonalidade
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Mes { get; set; }
        public string? Descricao { get; set; }

        public Produto Produto { get; set; } = null!;
    }
}
