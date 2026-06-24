using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class ProdutoSazonalidadeMap : IEntityTypeConfiguration<ProdutoSazonalidade>
    {
        public void Configure(EntityTypeBuilder<ProdutoSazonalidade> builder)
        {
            builder.ToTable("ProdutoSazonalidades");

            builder.HasKey(ps => ps.Id);

            builder.Property(ps => ps.Mes)
                .IsRequired();


            builder.HasOne(ps => ps.Produto)
                .WithMany(p => p.ProdutoSazonalidades)
                .HasForeignKey(ps => ps.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}