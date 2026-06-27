using System.ComponentModel.DataAnnotations;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class CategoriaCreateDto
    {
        [Required(ErrorMessage = "O id da unidade é obrigatório.")]
        public int UnidadeId { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da categoria não pode passar de 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;
    }
}