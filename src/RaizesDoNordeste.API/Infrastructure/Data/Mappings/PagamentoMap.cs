using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class PagamentoMap : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.ToTable("Pagamentos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Valor)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.FormaPagamento)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<int>() 
                .IsRequired();

            builder.Property(p => p.GatewayTransacaoId)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(p => p.GatewayPayloadRetorno)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property(p => p.DataCriacao)
                .IsRequired();

           
            builder.HasOne(p => p.Pedido)
                .WithMany(ped => ped.Pagamentos) 
                .HasForeignKey(p => p.PedidoId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}