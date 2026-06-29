namespace RaizesDoNordeste.API.Application.DTOs
{
    public class ProdutoResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string UrlImagem { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int UnidadeId { get; set; }
        public bool Ativo { get; set; }
        public bool IsSazonal { get; set; }
    }
}