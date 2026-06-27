using System.ComponentModel.DataAnnotations;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class PedidoStatusUpdateDto
    {
        [Required(ErrorMessage = "O novo status do pedido é obrigatório.")]
        public StatusPedido NovoStatus { get; set; }
    }
}