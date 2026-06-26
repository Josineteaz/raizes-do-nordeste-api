using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Domain.Entities
{
    public class Promocao
    {
        public int Id { get; set; }
        public int UnidadeId { get; set; }
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public TipoDesconto TipoDesconto { get; set; }
        public decimal ValorDesconto { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public Unidade Unidade { get; set; } = null!;
        public Produto Produto { get; set; } = null!;
    }
}
