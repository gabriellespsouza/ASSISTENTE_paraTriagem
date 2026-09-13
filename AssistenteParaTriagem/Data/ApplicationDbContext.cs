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

        public DbSet<AvaliacaoTriagem> Avaliacoes { get; set; }

        public DbSet<CenarioClinico> CenariosClinicos { get; set; }

        public DbSet<AvaliacaoCenario> AvaliacoesCenarios { get; set; }

        public DbSet<RespostaQuestionario> RespostasQuestionarios { get; set; }

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

            builder.Entity<AvaliacaoCenario>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.CenarioClinico)
                    .WithMany()
                    .HasForeignKey(e => e.CenarioClinicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.NomeProfissional)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            builder.Entity<CenarioClinico>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.QueixaPrincipal)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Sintomas)
                    .HasMaxLength(2000);

                entity.Property(e => e.DiscriminadoresEsperados)
                    .HasMaxLength(2000);
            });

            builder.Entity<RespostaQuestionario>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.NomeProfissional)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.PontosPositivos)
                    .HasMaxLength(2000);

                entity.Property(e => e.Dificuldades)
                    .HasMaxLength(2000);

                entity.Property(e => e.Sugestoes)
                    .HasMaxLength(2000);
            });
        }
    }
}