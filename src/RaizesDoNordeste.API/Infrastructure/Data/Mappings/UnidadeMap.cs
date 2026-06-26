using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class UnidadeMap : IEntityTypeConfiguration<Unidade>
    {
        public void Configure(EntityTypeBuilder<Unidade> builder)
        {
            builder.ToTable("Unidades");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nome)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Cnpj)
                .HasMaxLength(14) 
                .IsRequired();

            builder.Property(u => u.Telefone)
                .HasMaxLength(15) 
                .IsRequired();

            builder.Property(u => u.Cep)
                .HasMaxLength(8) 
                .IsRequired();

            builder.Property(u => u.Logradouro)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.Numero)
                .HasMaxLength(10)
                .IsRequired(false); 

            builder.Property(u => u.Bairro)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.Cidade)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.Estado)
                .HasMaxLength(2) 
                .IsRequired();

            builder.Property(u => u.TipoCozinha)
                .HasConversion<int>() 
                .IsRequired();

            builder.Property(u => u.HorarioAbertura)
                .IsRequired();

            builder.Property(u => u.HorarioFechamento)
                .IsRequired();


            builder.Property(u => u.TaxaEntrega)
                .HasColumnType("decimal(10,2)")
                .IsRequired(false);


            builder.HasMany(u => u.Pedidos)
                .WithOne(p => p.Unidade) 
                .HasForeignKey(p => p.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Produtos)
                .WithOne(p => p.Unidade)
                .HasForeignKey(p => p.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(u => u.EstoqueMovimentos)
                .WithOne(em => em.Unidade) 
                .HasForeignKey(em => em.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}