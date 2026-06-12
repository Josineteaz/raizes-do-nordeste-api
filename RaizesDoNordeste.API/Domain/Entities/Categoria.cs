namespace RaizesDoNordeste.API.Domain.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        public int UnidadeId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        public Unidade Unidade { get; set; } = null!;
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
