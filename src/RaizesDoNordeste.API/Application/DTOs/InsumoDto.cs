using System.ComponentModel.DataAnnotations;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public record InsumoCreateDto(
        [Required] int UnidadeId,
        [Required][StringLength(100)] string Nome,
        [Required][StringLength(10)] string UnidadeMedida,
        [Range(0, 999999)] decimal QuantidadeMinima,
        [Range(0, 999999)] decimal QuantidadeAtual
    );

    public record InsumoResponseDto(
        int Id,
        int UnidadeId,
        string Nome,
        string UnidadeMedida,
        decimal QuantidadeMinima,
        decimal QuantidadeAtual,
        bool Ativo,
        bool NecessitaReabastecimento
    );
}