using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Domain.Entities
{
    public class Usuario
    {
        // Campos utilizados por Clientes e Funcionários
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public DateTime DataInclusao { get; set; }        
        public Perfil Perfil { get; set; }

        // Campos específicos para Funcionários 
        public int? UnidadeId { get; set; }

        // Campos específicos para Clientes
        public string? Cpf { get; set; }
        public DateOnly? DataNascimento { get; set; }
        public string? Telefone { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Referencia { get; set; }
        public bool ConsentimentoLgpd { get; set; }
        public DateTime? DataConsentimento { get; set; }
        public int? SaldoPontos { get; set; }
        public bool Ativo { get; set; } = true;


        public Unidade? Unidade { get; set; }
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public ICollection<FidelidadeMovimento> FidelidadeMovimentos { get; set; } = new List<FidelidadeMovimento>();
    }
}