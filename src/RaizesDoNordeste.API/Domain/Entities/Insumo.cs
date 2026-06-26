namespace RaizesDoNordeste.API.Domain.Entities
{
    public class Insumo
    {
        public int Id { get; set; }
        public int UnidadeId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string UnidadeMedida { get; set; } = string.Empty;
        public decimal QuantidadeMinima { get; set; }
        public decimal QuantidadeAtual { get; set; }
        public bool Ativo { get; set; } = true;

        public Unidade Unidade { get; set; } = null!;
        public ICollection<EstoqueMovimento> EstoqueMovimentos { get; set; } = new List<EstoqueMovimento>();
        public ICollection<ProdutoFichaTecnica> FichasTecnicas { get; set; } = new List<ProdutoFichaTecnica>();
    }
}
