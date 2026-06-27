
namespace RaizesDoNordeste.API.Application.DTOs
{
    public class PedidoResultadoDto
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string TipoErro { get; set; } = string.Empty;
        public long PedidoId { get; set; }
        public PedidoResponseDto? Dados { get; set; }

        public static PedidoResultadoDto SucessoResultado(long pedidoId, PedidoResponseDto dados) => new()
        {
            Sucesso = true,
            PedidoId = pedidoId,
            Dados = dados
        };

        public static PedidoResultadoDto Falha(string tipoErro, string mensagem) => new()
        {
            Sucesso = false,
            TipoErro = tipoErro,
            Mensagem = mensagem
        };
    }
}