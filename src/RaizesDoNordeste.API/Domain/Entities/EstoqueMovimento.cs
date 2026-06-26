using System;
using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Domain.Entities
{
    public class EstoqueMovimento
    {
        public long Id { get; set; }
        public int UnidadeId { get; set; }
        public int InsumoId { get; set; }
        public TipoMovimentoEstoque TipoMovimento { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime DataMovimento { get; set; }
        public string? Observacao { get; set; }


        public Unidade Unidade { get; set; } = null!;
        public Insumo Insumo { get; set; } = null!;
    }
}