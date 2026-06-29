using System;
using System.Threading.Tasks;
using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IAuditoriaRepository _auditoriaRepository;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IAuditoriaRepository auditoriaRepository) 
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _auditoriaRepository = auditoriaRepository;
        }

        public async Task<LoginResponseDto?> AutenticarAsync(LoginDto loginDto)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(loginDto.Email);

            if (usuario == null)
            {
                await GravarLogAuditoriaAsync("LOGIN_FALHA", "Usuario", null,
                    "Tentativa de login falhou. E-mail não encontrado.", loginDto.UnidadeId);
                return null;
            }

            var senhaValida = _passwordHasher.VerifyPassword(loginDto.Senha, usuario.SenhaHash);

            if (!senhaValida)
            {
                await GravarLogAuditoriaAsync("LOGIN_FALHA", "Usuario", null,
                    $"Tentativa de login falhou. Senha incorreta para o usuário ID: {usuario.Id}.", loginDto.UnidadeId);
                return null;
            }

            string token = _tokenService.GenerateToken(usuario);

            await GravarLogAuditoriaAsync("LOGIN_SUCESSO", "Usuario", usuario.Id,
                $"Usuário autenticado com sucesso no sistema. Perfil: {usuario.Perfil}.", loginDto.UnidadeId);

            return new LoginResponseDto
            {
                Token = token,
                Role = usuario.Perfil.ToString()
            };
        }

       
        public async Task RegistrarLogoutAsync(int? usuarioId)
        {
            await GravarLogAuditoriaAsync(
                acao: "LOGOUT",
                entidade: "Usuario",
                registroId: usuarioId,
                descricao: $"Usuário ID {usuarioId} efetuou logout voluntário.",
                unidadeId: null
            );
        }

        private async Task GravarLogAuditoriaAsync(string acao, string entidade, long? registroId, string descricao, int? unidadeId)
        {
            var log = new Auditoria
            {
                Acao = acao,
                Entidade = entidade,
                RegistroId = registroId ?? 0,
                DataAcao = DateTime.UtcNow,
                Descricao = descricao,
                UsuarioId = registroId.HasValue ? (int)registroId.Value : null,
                UnidadeId = (unidadeId == 0) ? null : unidadeId
            };

           
            await _auditoriaRepository.SalvarAuditoriaAsync(log);
        }
    }
}