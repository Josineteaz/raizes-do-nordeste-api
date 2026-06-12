namespace RaizesDoNordeste.API.Domain.Entities
{
    public class ProdutoFichaTecnica
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int InsumoId { get; set; }
        public decimal Quantidade { get; set; }
        public bool AparecerMenu { get; set; }


        public Produto Produto { get; set; } = null!;
        public Insumo Insumo { get; set; } = null!;
    }
}
