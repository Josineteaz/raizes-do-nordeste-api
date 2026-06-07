using System;
using System.Collections.Generic;
using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Domain.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public int UnidadeId { get; set; }     // UnidadeId (Chave estrangeira para Unidade)
        public int ClienteId { get; set; } // ClienteId (Chave estrangeira para Usuário)
        public int NumPedidoUnidade { get; set; } // Número do Pedido por Unidade
        public CanalPedido Canal { get; set; } // CanalPedido [Enum]
        public ModalidadePedido Modalidade { get; set; } // ModalidadePedido [Enum]
        public DateTime DataPedido { get; set; }
        public StatusPedido Status { get; set; } // StatusPedido [Enum]
        public decimal ValorBruto { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal TaxaEntrega { get; set; }
        public decimal ValorTotal { get; set; }  
        public string? Observacao { get; set; }


        public Usuario Cliente { get; set; } = null!;
        public Unidade Unidade { get; set; } = null!;
        public ICollection<PedidoItem> Items { get; set; } = new List<PedidoItem>();
    }
}