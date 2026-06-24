using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RaizesDoNordeste.API.Domain.Entities;

namespace RaizesDoNordeste.API.Infrastructure.Data.Mappings
{
    public class AuditoriaMap : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.ToTable("Auditorias");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Acao)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(a => a.Entidade)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.Descricao)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(a => a.DataAcao)
                .IsRequired();

            builder.Property(a => a.RegistroId)
                .IsRequired();

            builder.HasOne(a => a.Unidade)
                .WithMany()
                .HasForeignKey(a => a.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}