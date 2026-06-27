using System.ComponentModel.DataAnnotations;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class ProdutoCreateDto
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode passar de 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O preço do produto é obrigatório.")]
        [Range(0.01, 1000.00, ErrorMessage = "O preço deve ser entre R$ 0,01 e R$ 1.000,00.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O ID da categoria é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria válida.")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "O ID da unidade/franquia é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma unidade válida.")]
        public int UnidadeId { get; set; }

        public string UrlImagem { get; set; } = string.Empty;

        public bool ESazonal { get; set; }

        public List<int> MesesSazonais { get; set; } = new List<int>();

        public string? DescricaoSazonal { get; set; }
    }
}