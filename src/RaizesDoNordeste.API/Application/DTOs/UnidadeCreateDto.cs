using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class UnidadeCreateDto
    {
        public string Cnpj { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string? Numero { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public TipoCozinha TipoCozinha { get; set; }
        public TimeOnly HorarioAbertura { get; set; }
        public TimeOnly HorarioFechamento { get; set; }
        public decimal? TaxaEntrega { get; set; }
    }
}