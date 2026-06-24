using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class PromocaoMap : IEntityTypeConfiguration<Promocao>
    {
        public void Configure(EntityTypeBuilder<Promocao> builder)
        {
            builder.ToTable("Promocoes");

            builder.HasKey(pr => pr.Id);

            builder.Property(pr => pr.Nome)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(pr => pr.Descricao)
                .HasMaxLength(250)
                .IsRequired(false);

            builder.Property(pr => pr.TipoDesconto)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(pr => pr.ValorDesconto)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(pr => pr.DataInicio)
                .IsRequired();

            builder.Property(pr => pr.DataFim)
                .IsRequired();

            builder.HasOne<Unidade>()
                .WithMany()
                .HasForeignKey(pr => pr.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Produto>()
                .WithMany()
                .HasForeignKey(pr => pr.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}