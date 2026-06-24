using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class FidelidadeMovimentoMap : IEntityTypeConfiguration<FidelidadeMovimento>
    {
        public void Configure(EntityTypeBuilder<FidelidadeMovimento> builder)
        {
            builder.ToTable("FidelidadeMovimentos");

            builder.HasKey(fm => fm.Id);

            builder.Property(fm => fm.TipoMovimento)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(fm => fm.Pontos)
                .IsRequired();

            builder.HasOne(fm => fm.Cliente)
                .WithMany(u => u.FidelidadeMovimentos)
                .HasForeignKey(fm => fm.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fm => fm.Pedido)
                .WithMany(p => p.FidelidadeMovimentos) 
                .HasForeignKey(fm => fm.PedidoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}