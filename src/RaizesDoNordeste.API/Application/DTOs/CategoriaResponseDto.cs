namespace RaizesDoNordeste.API.Application.DTOs
{
    public class CategoriaResponseDto
    {
        public int Id { get; set; }
        public int UnidadeId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }
}