using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class InsumoMap : IEntityTypeConfiguration<Insumo>
    {
        public void Configure(EntityTypeBuilder<Insumo> builder)
        {
            builder.ToTable("Insumos");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Nome)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(i => i.UnidadeMedida)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(i => i.QuantidadeAtual)
                .HasColumnType("decimal(18,4)")
                .IsRequired();

            builder.Property(i => i.QuantidadeMinima)
                .HasColumnType("decimal(18,4)")
                .IsRequired();


            builder.HasOne(i => i.Unidade)
                .WithMany() 
                .HasForeignKey(i => i.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.EstoqueMovimentos)
                .WithOne(em => em.Insumo)
                .HasForeignKey(em => em.InsumoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}