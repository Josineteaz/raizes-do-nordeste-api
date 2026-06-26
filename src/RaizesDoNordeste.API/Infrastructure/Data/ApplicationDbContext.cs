using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RaizesDoNordeste.API.Domain.Entities;


namespace RaizesDoNordeste.API.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Unidade> Unidades { get; set; }
        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Produto> Produtos { get; set; }
        public virtual DbSet<Insumo> Insumos { get; set; }
        public virtual DbSet<Pedido> Pedidos { get; set; }
        public virtual DbSet<PedidoItem> PedidoItens { get; set; }
        public virtual DbSet<EstoqueMovimento> EstoqueMovimentos { get; set; }
        public virtual DbSet<FidelidadeMovimento> FidelidadeMovimentos { get; set; }
        public virtual DbSet<ProdutoFichaTecnica> ProdutoFichasTecnicas { get; set; }
        public virtual DbSet<Promocao> Promocoes { get; set; }
        public virtual DbSet<Auditoria> Auditorias { get; set; }
        public virtual DbSet<Pagamento> Pagamentos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int? usuarioLogadoId = ObterUsuarioIdDoToken();

            var alteracoes = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            var registrosAuditoria = new List<Auditoria>();

            var entidadesNovas = new List<(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entrada, Auditoria Auditoria)>();

            foreach (var entrada in alteracoes)
            {
                if (entrada.Entity is Auditoria) continue;

                int? unidadeIdDetectada = null;
                var possuiPropriedadeUnidade = entrada.Properties.Any(p => p.Metadata.Name == "UnidadeId");
                if (possuiPropriedadeUnidade)
                {
                    var valorAtual = entrada.Property("UnidadeId").CurrentValue;
                    if (valorAtual != null)
                    {
                        int idConvertido = Convert.ToInt32(valorAtual);
                        unidadeIdDetectada = (idConvertido == 0) ? null : idConvertido;
                    }
                }

                long idAtual = Convert.ToInt64(entrada.Property("Id").CurrentValue);
                string nomeEntidade = entrada.Entity.GetType().Name;

                string descricaoAuditoria = "";

                if (nomeEntidade == "Pedido" || entrada.Entity is Pedido)
                {

                    if (entrada.State == EntityState.Added)
                    {
                        descricaoAuditoria = "Pedido criado no sistema.";
                    }
                    else if (entrada.State == EntityState.Modified)
                    {
                        var propStatus = entrada.Property("Status");

                        if (propStatus.IsModified)
                        {
                            var statusAntigo = propStatus.OriginalValue?.ToString();
                            var statusNovo = propStatus.CurrentValue?.ToString();

                            if (statusNovo == "Cancelado")
                            {
                                descricaoAuditoria = $"Pedido Cancelado. Status alterado de '{statusAntigo}' para '{statusNovo}'.";
                            }
                            else
                            {
                                descricaoAuditoria = $"Alteração de status do pedido de '{statusAntigo}' para '{statusNovo}'.";
                            }
                        }
                        else
                        {
                            descricaoAuditoria = "Pedido modificado.";
                        }
                    }
                    else if (entrada.State == EntityState.Deleted)
                    {
                        descricaoAuditoria = "Pedido excluído do sistema.";
                    }
                }
                else
                {
                    descricaoAuditoria = entrada.State == EntityState.Added
                        ? $"Novo registro da entidade {nomeEntidade} cadastrado no sistema."
                        : $"A entidade {nomeEntidade} foi modificada no estado {entrada.State}.";
                }

                var auditoria = new Auditoria
                {
                    DataAcao = DateTime.UtcNow,
                    Acao = entrada.State.ToString().ToUpper(),
                    Entidade = nomeEntidade,
                    RegistroId = idAtual < 0 ? 0 : idAtual, 
                    Descricao = descricaoAuditoria,
                    UsuarioId = usuarioLogadoId,
                    UnidadeId = unidadeIdDetectada
                };

                registrosAuditoria.Add(auditoria);

                if (entrada.State == EntityState.Added)
                {
                    entidadesNovas.Add((entrada, auditoria));
                }
            }

            int resultado = await base.SaveChangesAsync(cancellationToken);

            if (registrosAuditoria.Any())
            {
                foreach (var item in entidadesNovas)
                {
                    long idGerado = Convert.ToInt64(item.Entrada.Property("Id").CurrentValue);
                    item.Auditoria.RegistroId = idGerado;

                    if (item.Auditoria.Entidade == "Pedido")
                    {
                        item.Auditoria.Descricao = $"Pedido id {idGerado} criado no sistema.";
                    }
                }

                await Auditorias.AddRangeAsync(registrosAuditoria, cancellationToken);
                await base.SaveChangesAsync(cancellationToken);
            }

            return resultado;
        }

        private int? ObterUsuarioIdDoToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {

                var claimId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? httpContext.User.FindFirst("id")?.Value;

                if (int.TryParse(claimId, out int id))
                {
                    return id;
                }
            }

            return null;
        }
    }
}