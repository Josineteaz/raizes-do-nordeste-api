using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RaizesDoNordeste.API.Application;
using RaizesDoNordeste.API.Application.Services;
using RaizesDoNordeste.API.Infrastructure;
using RaizesDoNordeste.API.Infrastructure.Data;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// REGISTRO DE SERVIÇOS DO SISTEMA
builder.Services.AddApplicationHierarchy();
builder.Services.AddInfrastructureHierarchy();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddEndpointsApiExplorer();

// REGISTRO DO WORKER DE LIMPEZA DE ESTOQUE/PEDIDOS
builder.Services.AddHostedService<CancelamentoPedidoWorker>();

// CONFIGURAÇÃO DO BANCO DE DADOS
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// CONFIGURAÇÃO DE AUTENTICAÇÃO JWT
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];

    if (string.IsNullOrEmpty(jwtSecretKey))
    {
        throw new InvalidOperationException("A chave secreta do JWT não foi configurada no appsettings.json.");
    }

    var keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role
    };
});

// CONFIGURAÇÃO DO SWAGGER
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Raízes do Nordeste API",
        Version = "v1",
        Description = "Documentação oficial da API desenvolvida para Raízes do Nordeste."
    });

    c.ExampleFilters();

    // Configura o Swagger para ler os comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Define o esquema de segurança (Bearer)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta forma: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // CONFIGURAÇÃO DO REQUERIMENTO DE SEGURANÇA
    var securityRequirement = new OpenApiSecurityRequirement();

    var schemeReference = new OpenApiSecuritySchemeReference("Bearer");

    var scopes = new List<string>();

    securityRequirement.Add(schemeReference, scopes);

    c.AddSecurityRequirement(doc => securityRequirement);
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Raízes do Nordeste API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});

app.Run();