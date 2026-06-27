using System.ComponentModel.DataAnnotations;
using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class EstoqueMovimentoDto
    {
        [Required(ErrorMessage = "O ID do insumo é obrigatório.")]
        public int InsumoId { get; set; }

        [Required(ErrorMessage = "O ID da unidade é obrigatório.")]
        public int UnidadeId { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        public decimal Quantidade { get; set; }

        [Required(ErrorMessage = "O tipo de movimentação é obrigatório.")]
        public TipoMovimentoEstoque TipoMovimento { get; set; }

        public string? Observacao { get; set; }
    }
}