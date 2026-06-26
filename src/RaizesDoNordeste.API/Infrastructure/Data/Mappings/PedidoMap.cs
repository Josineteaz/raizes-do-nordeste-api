using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class PedidoMap : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.ClienteId)
                .IsRequired(false);

            builder.Property(p => p.NumPedidoUnidade)
                .IsRequired();

            builder.Property(p => p.CanalPedido)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.Modalidade)
                .HasConversion<int>()
                .IsRequired(false);

            builder.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.DataPedido)
                .IsRequired();

            builder.Property(p => p.Observacao)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.ValorPago)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.TaxaEntrega)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);

            builder.Property(p => p.ValorDesconto)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);


            builder.HasOne(p => p.Unidade)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(p => p.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Cliente)
                .WithMany()
                .HasForeignKey(p => p.ClienteId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Itens)
                .WithOne(i => i.Pedido)
                .HasForeignKey(i => i.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.FidelidadeMovimentos)
                .WithOne(fm => fm.Pedido)
                .HasForeignKey(fm => fm.PedidoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}