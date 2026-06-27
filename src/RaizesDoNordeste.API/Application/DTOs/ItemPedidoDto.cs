using System.ComponentModel.DataAnnotations;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class ItemPedidoDto
    {
        [Required(ErrorMessage = "O ID do produto é obrigatório.")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(1, 100, ErrorMessage = "A quantidade deve ser entre {1} e {2}.")]
        public int Quantidade { get; set; }
    }
}