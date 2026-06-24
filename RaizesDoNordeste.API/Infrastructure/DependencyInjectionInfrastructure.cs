using RaizesDoNordeste.API.Domain.Interfaces;
using RaizesDoNordeste.API.Infrastructure.Data.Repositories;
using RaizesDoNordeste.API.Infrastructure.Repositories;
using RaizesDoNordeste.API.Infrastructure.Security;

namespace RaizesDoNordeste.API.Infrastructure
{
    public static class DependencyInjectionInfrastructure
    {
        public static IServiceCollection AddInfrastructureHierarchy(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();            
            services.AddScoped<IUnidadeRepository, UnidadeRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IEstoqueMovimentoRepository, EstoqueMovimentoRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IFidelidadeRepository, FidelidadeRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IInsumoRepository, InsumoRepository>();
            services.AddScoped<IProdutoFichaTecnicaRepository, ProdutoFichaTecnicaRepository>();
            services.AddScoped<IPromocaoRepository, PromocaoRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

            return services;
        }
    }
}