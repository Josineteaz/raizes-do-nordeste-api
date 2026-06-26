using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nome)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.SenhaHash)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.DataInclusao)
                .IsRequired();

            builder.Property(u => u.Perfil)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(u => u.Cpf)
                .HasMaxLength(11)
                .IsRequired(false);

            builder.Property(u => u.Telefone)
                .HasMaxLength(15)
                .IsRequired(false);

            builder.Property(u => u.SaldoPontos)
                .HasDefaultValue(0)
                .IsRequired(false);

            builder.Property(u => u.Cep).HasMaxLength(8).IsRequired(false);
            builder.Property(u => u.Logradouro).HasMaxLength(150).IsRequired(false);
            builder.Property(u => u.Numero).HasMaxLength(10).IsRequired(false);
            builder.Property(u => u.Bairro).HasMaxLength(50).IsRequired(false);
            builder.Property(u => u.Cidade).HasMaxLength(50).IsRequired(false);
            builder.Property(u => u.Estado).HasMaxLength(2).IsRequired(false);
            builder.Property(u => u.Referencia).HasMaxLength(150).IsRequired(false);

            builder.Property(u => u.ConsentimentoLgpd)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.DataConsentimento)
                .IsRequired(false);

            builder.HasOne(u => u.Unidade)
                .WithMany() 
                .HasForeignKey(u => u.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); 

            builder.HasMany(u => u.Pedidos)
                .WithOne(p => p.Cliente)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.FidelidadeMovimentos)
                .WithOne(fm => fm.Cliente)
                .HasForeignKey(fm => fm.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}