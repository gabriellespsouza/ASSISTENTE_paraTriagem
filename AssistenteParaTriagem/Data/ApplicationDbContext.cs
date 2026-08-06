using AssistenteParaTriagem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssistenteParaTriagem.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Histórico das triagens (log de auditoria)
        public DbSet<AvaliacaoTriagem> Avaliacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AvaliacaoTriagem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.NomeProfissional)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Queixa)
                      .HasMaxLength(500);

                entity.Property(e => e.Sintomas)
                      .HasMaxLength(2000);

                entity.Property(e => e.Discriminadores)
                      .HasMaxLength(3000);

                entity.Property(e => e.RegrasAplicadas)
                      .HasMaxLength(2000);

                entity.Property(e => e.Justificativa)
                      .HasMaxLength(2000);
            });
        }
    }
}
