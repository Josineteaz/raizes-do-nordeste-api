using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Application.Services;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Application
{
    public static class DependencyInjectionApplication
    {
        public static IServiceCollection AddApplicationHierarchy(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUnidadeService, UnidadeService>(); 
            services.AddScoped<IFidelidadeService, FidelidadeService>();
            services.AddScoped<IPromocaoService, PromocaoService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<IEstoqueMovimentoService, EstoqueMovimentoService>();
            services.AddScoped<IPagamentoService, PagamentoService>();
            services.AddScoped<IPedidoService, PedidoService>();

            services.AddScoped<IAuditoriaService, AuditoriaService>();

            return services;
        }
    }
}