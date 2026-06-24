using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class ProdutoFichaTecnicaMap : IEntityTypeConfiguration<ProdutoFichaTecnica>
    {
        public void Configure(EntityTypeBuilder<ProdutoFichaTecnica> builder)
        {
            builder.ToTable("ProdutoFichasTecnicas");

            builder.HasKey(pft => pft.Id);

            builder.Property(pft => pft.Quantidade)
                .HasColumnType("decimal(18,4)")
                .IsRequired();

            builder.Property(pft => pft.AparecerMenu)
                .IsRequired()
                .HasDefaultValue(true);


            builder.HasOne(pft => pft.Produto)
                .WithMany(p => p.FichasTecnicas)
                .HasForeignKey(pft => pft.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pft => pft.Insumo)
                .WithMany(i => i.FichasTecnicas)
                .HasForeignKey(pft => pft.InsumoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}