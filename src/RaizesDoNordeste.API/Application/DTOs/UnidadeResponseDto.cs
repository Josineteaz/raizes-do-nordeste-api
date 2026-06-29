namespace RaizesDoNordeste.API.Application.DTOs
{
    public class UnidadeResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool PossuiCozinhaCompleta { get; set; }
        public string HorarioAbertura { get; set; } = string.Empty;
        public string HorarioFechamento { get; set; } = string.Empty;
        public decimal? TaxaEntrega { get; set; }

        public string Cep { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string? Numero { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}