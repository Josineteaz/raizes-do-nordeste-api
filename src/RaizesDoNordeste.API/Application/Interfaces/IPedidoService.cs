using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Application.Interfaces
{
    public interface IPedidoService
    {

        Task<PedidoResultadoDto> CriarPedidoAsync(PedidoCreateDto pedidoDto);

        Task<PedidoResponseDto?> ObterPedidoPorIdAsync(long id);

        Task<IEnumerable<PedidoResponseDto>> ObterPedidosAsync(int? unidadeId, string? canalPedido, int page = 1, int limit = 10);

        Task<bool> AtualizarStatusAsync(long id, StatusPedido novoStatus);

        Task<IEnumerable<PedidoFilaProducaoDto>> ObterPedidosPagosPorUnidadeAsync(int unidadeId, int page = 1, int limit = 10);

        Task<IEnumerable<PedidoFilaBalcaoDto>> ObterPedidosProntosPorUnidadeAsync(int unidadeId, int page = 1, int limit = 10);
    }
}