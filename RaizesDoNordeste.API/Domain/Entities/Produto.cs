namespace RaizesDoNordeste.API.Domain.Entities
{
    public class Produto
    {
        public int Id { get; set; }
        public int UnidadeId { get; set; }
        public int CategoriaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string UrlImagem { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        public Unidade Unidade { get; set; } = null!;
        public Categoria Categoria { get; set; } = null!;
        public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
        public ICollection<ProdutoSazonalidade> ProdutoSazonalidades { get; set; } = new List<ProdutoSazonalidade>();
        public ICollection<ProdutoFichaTecnica> FichasTecnicas { get; set; } = new List<ProdutoFichaTecnica>();
    }
}