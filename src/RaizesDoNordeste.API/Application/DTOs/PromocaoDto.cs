using System;
using System.ComponentModel.DataAnnotations;
using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class PromocaoDto
    {
        [Required(ErrorMessage = "O ID da unidade é obrigatório.")]
        public int UnidadeId { get; set; }

        [Required(ErrorMessage = "O ID do produto é obrigatório.")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "O nome da promoção é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O tipo de desconto é obrigatório.")]
        public TipoDesconto TipoDesconto { get; set; }

        [Required(ErrorMessage = "O valor do desconto é obrigatório.")]
        [Range(0.01, 999999.99, ErrorMessage = "O valor do desconto deve ser maior que zero.")]
        public decimal ValorDesconto { get; set; }

        [Required(ErrorMessage = "A data de início é obrigatória.")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "A data de término é obrigatória.")]
        public DateTime DataFim { get; set; }
    }
}