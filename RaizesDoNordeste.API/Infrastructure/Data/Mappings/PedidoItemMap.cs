using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class PedidoItemMap : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {
            builder.ToTable("PedidoItens");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.Quantidade)
                .IsRequired();

            builder.Property(pi => pi.PrecoUnitario)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.HasOne(pi => pi.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(pi => pi.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pi => pi.Produto)
                .WithMany(p => p.Itens)
                .HasForeignKey(pi => pi.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}