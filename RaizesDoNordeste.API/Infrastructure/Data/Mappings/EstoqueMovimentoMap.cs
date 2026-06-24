using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class EstoqueMovimentoMap : IEntityTypeConfiguration<EstoqueMovimento>
    {
        public void Configure(EntityTypeBuilder<EstoqueMovimento> builder)
        {
            builder.ToTable("EstoqueMovimentos");

            builder.HasKey(em => em.Id);

            builder.Property(em => em.TipoMovimento)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(em => em.Quantidade)
                .HasColumnType("decimal(18,4)") 
                .IsRequired();

            builder.Property(em => em.DataMovimento)
                .IsRequired();

            builder.Property(em => em.Observacao)
                .HasMaxLength(250)
                .IsRequired(false);


            builder.HasOne(em => em.Unidade)
                .WithMany(u => u.EstoqueMovimentos)
                .HasForeignKey(em => em.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(em => em.Insumo)
                .WithMany(i => i.EstoqueMovimentos)
                .HasForeignKey(em => em.InsumoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}