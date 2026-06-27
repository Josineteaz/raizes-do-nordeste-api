namespace RaizesDoNordeste.API.Application.DTOs
{
    public class AuditoriaResponseDto
    {
        public long Id { get; set; }
        public int? UnidadeId { get; set; }
        public int? UsuarioId { get; set; }
        public DateTime DataAcao { get; set; }
        public string Acao { get; set; } = string.Empty;
        public string Entidade { get; set; } = string.Empty;
        public long RegistroId { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}