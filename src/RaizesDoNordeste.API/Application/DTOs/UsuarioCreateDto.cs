using System;
using RaizesDoNordeste.API.Domain.Enums;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class UsuarioCreateDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public Perfil Perfil { get; set; }
        public bool ConsentimentoLgpd { get; set; }
        public int? UnidadeId { get; set; }
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
    }

    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; }

        public int? UnidadeId { get; set; }
        public string? Cpf { get; set; }
        public string? DataNascimento { get; set; }
        public string? Telefone { get; set; }

        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Referencia { get; set; }

        public int SaldoPontos { get; set; }
    }
}