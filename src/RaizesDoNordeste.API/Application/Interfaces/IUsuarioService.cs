using System.Collections.Generic;
using System.Threading.Tasks;
using RaizesDoNordeste.API.Application.DTOs;

namespace RaizesDoNordeste.API.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDto?> ObterPorIdAsync(int id);

        Task<IEnumerable<UsuarioResponseDto>> ObterTodosAsync();

        Task<IEnumerable<UsuarioResponseDto>> ObterTodosPaginadoAsync(int page, int limit, int? adminId);

        Task<UsuarioResponseDto?> CriarUsuarioAsync(UsuarioCreateDto usuarioDto);

        Task<bool> AtualizarUsuarioAsync(int id, UsuarioCreateDto usuarioDto);


        Task<bool> DesativarAsync(int id, bool solicitouEsquecimentoLgpd = false, int? executorId = null);
    }
}