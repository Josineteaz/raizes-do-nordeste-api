using System.ComponentModel.DataAnnotations;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class FichaTecnicaItemDto
    {
        [Required(ErrorMessage = "O identificador do insumo é obrigatório.")]
        public int InsumoId { get; set; }

        [Required(ErrorMessage = "A quantidade do insumo é obrigatória.")]
        [Range(0.001, 99999.999, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public decimal Quantidade { get; set; }

        [Required(ErrorMessage = "Indique se este insumo deve ser exibido no menu.")]
        public bool AparecerMenu { get; set; }
    }
}