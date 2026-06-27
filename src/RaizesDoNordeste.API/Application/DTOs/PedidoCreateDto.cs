using RaizesDoNordeste.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class PedidoCreateDto
    {
        public int? ClienteId { get; set; }

        [Required(ErrorMessage = "O ID da unidade (filial) é obrigatório.")]
        public int UnidadeId { get; set; }

        [Required(ErrorMessage = "O canal do pedido é obrigatório.")]
        [EnumDataType(typeof(CanalPedido), ErrorMessage = "Canal inválido. Permitidos: 1 (App), 2 (Web), 3 (Totem), 4 (Balcao).")]
        public int? CanalPedido { get; set; }

        [Required(ErrorMessage = "A modalidade do pedido é obrigatória.")]
        [Range(1, 2, ErrorMessage = "Modalidade inválida. Permitidos: 1 (Delivery), 2 (PickUp).")]
        public int? Modalidade { get; set; }

        [MinLength(1, ErrorMessage = "O pedido deve conter pelo menos um item.")]
        public List<ItemPedidoDto> Itens { get; set; } = new();

        public int? PontosParaUtilizar { get; set; }
        public string? Observacao { get; set; }
    }
}