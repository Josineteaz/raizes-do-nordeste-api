using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Domain.Entities
{
    public class Unidade
    {
        public int Id { get; set; }
        public string Cnpj { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string? Numero { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public TipoCozinha TipoCozinha { get; set; }
        public TimeOnly HorarioAbertura { get; set; }
        public TimeOnly HorarioFechamento { get; set; }
        public decimal? TaxaEntrega { get; set; }
        public bool Ativo { get; set; } = true;


        public ICollection<EstoqueMovimento> EstoqueMovimentos { get; set; } = new List<EstoqueMovimento>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
        public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
    }
}