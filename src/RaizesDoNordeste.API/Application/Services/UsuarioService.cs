using System.Security.Claims;
using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Enums;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditoriaRepository _auditoriaRepository;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            IHttpContextAccessor httpContextAccessor,
            IAuditoriaRepository auditoriaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _auditoriaRepository = auditoriaRepository;
        }


        public async Task<UsuarioResponseDto?> ObterPorIdAsync(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null) return null;

            var (idLogado, roleLogado) = ObterContextoUsuarioLogado();
            return MapearParaResponseDtoSeguro(usuario, roleLogado, idLogado);
        }

        public async Task<IEnumerable<UsuarioResponseDto>> ObterTodosPaginadoAsync(int page, int limit, int? adminId)
        {
            var usuarios = await _usuarioRepository.ObterTodosPaginadoAsync(page, limit);
            var (idLogado, roleLogado) = ObterContextoUsuarioLogado();

            var logAcesso = new Auditoria
            {
                Acao = "ACESSO_SENSIVEL",
                Entidade = "Usuario",
                RegistroId = 0,
                DataAcao = DateTime.UtcNow,
                Descricao = $"Administrador listou dados cadastrais. Página: {page}, Limite: {limit}.",
                UsuarioId = adminId,
                UnidadeId = null
            };
            await _auditoriaRepository.SalvarAuditoriaAsync(logAcesso);

            return usuarios.Select(u => MapearParaResponseDtoSeguro(u, roleLogado, idLogado)).ToList();
        }

        public async Task<IEnumerable<UsuarioResponseDto>> ObterTodosAsync()
        {
            var usuarios = await _usuarioRepository.ObterTodosAsync();
            var (idLogado, roleLogado) = ObterContextoUsuarioLogado();
            return usuarios.Select(u => MapearParaResponseDtoSeguro(u, roleLogado, idLogado)).ToList();
        }


        public async Task<UsuarioResponseDto?> CriarUsuarioAsync(UsuarioCreateDto usuarioDto)
        {
            var (idLogado, roleUsuarioLogado) = ObterContextoUsuarioLogado();

            var usuarioLogado = _httpContextAccessor.HttpContext?.User;
            var estaAutenticado = usuarioLogado?.Identity?.IsAuthenticated ?? false;

            var unidadeClaim = usuarioLogado?.FindFirst("UnidadeId")?.Value;
            int.TryParse(unidadeClaim, out int unidadeGerenteId);

            if (!estaAutenticado)
            {
                if (usuarioDto.Perfil != Perfil.Cliente)
                    throw new ArgumentException("Acesso negado. Visitantes não autenticados só podem criar perfis do tipo Cliente.");
            }
            else if (roleUsuarioLogado == "GerenteUnidade")
            {
                if (usuarioDto.UnidadeId != unidadeGerenteId)
                    throw new ArgumentException("Acesso negado. Você só pode cadastrar usuários para a sua própria unidade.");

                if (usuarioDto.Perfil == Perfil.GerenteUnidade || usuarioDto.Perfil == Perfil.AdministradorFranquia)
                    throw new ArgumentException("Acesso negado. Gerentes de unidade não possuem permissão para criar perfis administrativos.");
            }

            var existeUsuario = await _usuarioRepository.ObterPorEmailAsync(usuarioDto.Email);
            if (existeUsuario != null)
                throw new ArgumentException("O e-mail já está cadastrado no sistema.");

            string senhaHashSegura = _passwordHasher.HashPassword(usuarioDto.Senha);
            var usuarioEntidade = new Usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                SenhaHash = senhaHashSegura,
                Perfil = usuarioDto.Perfil,
                ConsentimentoLgpd = usuarioDto.ConsentimentoLgpd,
                DataInclusao = DateTime.UtcNow,
                DataConsentimento = usuarioDto.ConsentimentoLgpd ? DateTime.UtcNow : null,
                UnidadeId = usuarioDto.UnidadeId,
                Cpf = usuarioDto.Cpf,
                DataNascimento = usuarioDto.DataNascimento,
                Telefone = usuarioDto.Telefone,
                Cep = usuarioDto.Cep,
                Logradouro = usuarioDto.Logradouro,
                Numero = usuarioDto.Numero,
                Bairro = usuarioDto.Bairro,
                Cidade = usuarioDto.Cidade,
                Estado = usuarioDto.Estado,
                Referencia = usuarioDto.Referencia,
                SaldoPontos = 0
            };

            await _usuarioRepository.AdicionarAsync(usuarioEntidade);
            return MapearParaResponseDtoSeguro(usuarioEntidade, roleUsuarioLogado, idLogado);
        }

        public async Task<bool> AtualizarUsuarioAsync(int id, UsuarioCreateDto usuarioDto)
        {
            var usuarioAlvo = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuarioAlvo == null) return false;

            var (idUsuarioLogado, roleUsuarioLogado) = ObterContextoUsuarioLogado();

            var usuarioLogado = _httpContextAccessor.HttpContext?.User;
            var unidadeClaim = usuarioLogado?.FindFirst("UnidadeId")?.Value;
            int.TryParse(unidadeClaim, out int unidadeGerenteId);

           
            if (idUsuarioLogado == id)
            {
                usuarioDto.Perfil = usuarioAlvo.Perfil;
                usuarioDto.UnidadeId = usuarioAlvo.UnidadeId;
            }
            else if (roleUsuarioLogado == "GerenteUnidade")
            {
                if (usuarioAlvo.UnidadeId != unidadeGerenteId || usuarioDto.UnidadeId != unidadeGerenteId)
                    throw new ArgumentException("Acesso negado. Você não possui permissão para gerenciar usuários de outra unidade.");

                if (usuarioAlvo.Perfil == Perfil.GerenteUnidade || usuarioAlvo.Perfil == Perfil.AdministradorFranquia)
                    throw new ArgumentException("Acesso negado. Operação não permitida para o nível do usuário alvo.");

                if (usuarioDto.Perfil == Perfil.GerenteUnidade || usuarioDto.Perfil == Perfil.AdministradorFranquia)
                    throw new ArgumentException("Acesso negado. Você não pode promover usuários a cargos administrativos.");
            }
            else if (roleUsuarioLogado != "AdministradorFranquia")
            {
                return false;
            }

            usuarioAlvo.Nome = usuarioDto.Nome;
            usuarioAlvo.Email = usuarioDto.Email;
            usuarioAlvo.Perfil = usuarioDto.Perfil;
            usuarioAlvo.ConsentimentoLgpd = usuarioDto.ConsentimentoLgpd;
            usuarioAlvo.UnidadeId = usuarioDto.UnidadeId;
            usuarioAlvo.Cpf = usuarioDto.Cpf;
            usuarioAlvo.DataNascimento = usuarioDto.DataNascimento;
            usuarioAlvo.Telefone = usuarioDto.Telefone;
            usuarioAlvo.Cep = usuarioDto.Cep;
            usuarioAlvo.Logradouro = usuarioDto.Logradouro;
            usuarioAlvo.Numero = usuarioDto.Numero;
            usuarioAlvo.Bairro = usuarioDto.Bairro;
            usuarioAlvo.Cidade = usuarioDto.Cidade;
            usuarioAlvo.Estado = usuarioDto.Estado;
            usuarioAlvo.Referencia = usuarioDto.Referencia;

            if (!string.IsNullOrWhiteSpace(usuarioDto.Senha))
            {
                usuarioAlvo.SenhaHash = _passwordHasher.HashPassword(usuarioDto.Senha);
            }

            await _usuarioRepository.AtualizarAsync(usuarioAlvo);
            return true;
        }

        public async Task<bool> DesativarAsync(int id, bool solicitouEsquecimentoLgpd, int? executorId)
        {
            var usuarioAlvo = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuarioAlvo == null) return false;

            var (idUsuarioLogado, roleUsuarioLogado) = ObterContextoUsuarioLogado();

            var usuarioLogado = _httpContextAccessor.HttpContext?.User;
            var unidadeClaim = usuarioLogado?.FindFirst("UnidadeId")?.Value;
            int.TryParse(unidadeClaim, out int unidadeGerenteId);

            if (idUsuarioLogado != id)
            {
                if (roleUsuarioLogado == "GerenteUnidade")
                {
                    if (usuarioAlvo.UnidadeId != unidadeGerenteId)
                        throw new ArgumentException("Acesso negado. Este usuário pertence a outra unidade.");

                    if (usuarioAlvo.Perfil == Perfil.GerenteUnidade || usuarioAlvo.Perfil == Perfil.AdministradorFranquia)
                        throw new ArgumentException("Acesso negado. Não é possível remover usuários de nível gerencial.");
                }
                else if (roleUsuarioLogado != "AdministradorFranquia")
                {
                    return false;
                }
            }

            if (usuarioAlvo.Perfil == Perfil.Cliente && solicitouEsquecimentoLgpd)
            {
                usuarioAlvo.Nome = "CLIENTE_ANONIMIZADO_LGPD";
                usuarioAlvo.Email = $"anonimo_{usuarioAlvo.Id}@raizesdonordeste.com.br";
                usuarioAlvo.SenhaHash = "000000000000000000000000000000000";
                usuarioAlvo.Cpf = "00000000000";
                usuarioAlvo.DataNascimento = null;
                usuarioAlvo.Telefone = null;
                usuarioAlvo.Cep = null;
                usuarioAlvo.Logradouro = null;
                usuarioAlvo.Numero = null;
                usuarioAlvo.Bairro = null;
                usuarioAlvo.Cidade = null;
                usuarioAlvo.Estado = null;
                usuarioAlvo.ConsentimentoLgpd = false;
                usuarioAlvo.DataConsentimento = null;
                usuarioAlvo.SaldoPontos = 0;

                var logAuditoriaLgpd = new Auditoria
                {
                    Acao = "ANONIMIZACAO_LGPD",
                    Entidade = "Usuario",
                    RegistroId = id,
                    DataAcao = DateTime.UtcNow,
                    Descricao = $"Direito ao esquecimento executado com sucesso para o usuário ID {id}.",
                    UsuarioId = executorId,
                    UnidadeId = null
                };
                await _auditoriaRepository.SalvarAuditoriaAsync(logAuditoriaLgpd);
            }

            usuarioAlvo.Ativo = false;
            await _usuarioRepository.AtualizarAsync(usuarioAlvo);
            return true;
        }


        private (int Id, string Role) ObterContextoUsuarioLogado()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal == null || !(principal.Identity?.IsAuthenticated ?? false))
            {
                return (0, string.Empty);
            }

            var idStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst("sub")?.Value;
            int.TryParse(idStr, out int idLogado);

            var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            return (idLogado, role);
        }

        private static UsuarioResponseDto MapearParaResponseDtoSeguro(Usuario usuario, string perfilUsuarioLogado, int idUsuarioLogado)
        {
            bool podeVerDadosSensiveis = usuario.Id == idUsuarioLogado || perfilUsuarioLogado == "AdministradorFranquia";

            return new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                CriadoEm = usuario.DataInclusao,
                UnidadeId = usuario.UnidadeId,
                Cpf = podeVerDadosSensiveis ? usuario.Cpf : MaskCpf(usuario.Cpf),
                DataNascimento = podeVerDadosSensiveis ? usuario.DataNascimento?.ToString("dd/MM/yyyy") : "XX/XX/XXXX",
                Telefone = podeVerDadosSensiveis ? usuario.Telefone : MaskTelefone(usuario.Telefone),
                Cep = podeVerDadosSensiveis ? usuario.Cep : "XXXXX-XXX",
                Logradouro = podeVerDadosSensiveis ? usuario.Logradouro : "Protegido por LGPD",
                Numero = podeVerDadosSensiveis ? usuario.Numero : "***",
                Bairro = podeVerDadosSensiveis ? usuario.Bairro : "***",
                Cidade = usuario.Cidade,
                Estado = usuario.Estado,
                Referencia = podeVerDadosSensiveis ? usuario.Referencia : null,
                SaldoPontos = podeVerDadosSensiveis ? (usuario.SaldoPontos ?? 0) : 0
            };
        }

        private static string? MaskCpf(string? cpf) =>
            string.IsNullOrEmpty(cpf) || cpf.Length < 11 ? null : $"***.{cpf.Substring(3, 3)}..***-{cpf.Substring(cpf.Length - 2)}";

        private static string? MaskTelefone(string? tel) =>
            string.IsNullOrEmpty(tel) || tel.Length < 4 ? null : $"(**) *****-{tel.Substring(tel.Length - 4)}";
    }
}