using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class ProdutoMap : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Preco)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.UrlImagem)
                .HasMaxLength(300)
                .IsRequired(false);


            builder.HasOne(p => p.Unidade)
                .WithMany(u => u.Produtos)
                .HasForeignKey(p => p.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Itens)
                .WithOne(pi => pi.Produto)
                .HasForeignKey(pi => pi.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.ProdutoSazonalidades)
                .WithOne(ps => ps.Produto)
                .HasForeignKey(ps => ps.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.FichasTecnicas)
                .WithOne(pt => pt.Produto)
                .HasForeignKey(pt => pt.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}