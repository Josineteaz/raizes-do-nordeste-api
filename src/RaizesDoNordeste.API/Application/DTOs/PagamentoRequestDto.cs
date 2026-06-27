using System.ComponentModel.DataAnnotations;
using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class PagamentoRequestDto
    {
        [Required(ErrorMessage = "O ID do pedido é obrigatório para processar o pagamento.")]
        public long PedidoId { get; set; }       
 
        public StatusPagamento Status { get; set; }

    }
}