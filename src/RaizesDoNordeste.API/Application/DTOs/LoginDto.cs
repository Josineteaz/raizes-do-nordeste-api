using System.ComponentModel.DataAnnotations;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "O campo de e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo de senha é obrigatório.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
        public string Senha { get; set; } = string.Empty;

        public int? UnidadeId { get; set; }
    }
}