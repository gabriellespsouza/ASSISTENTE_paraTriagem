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



        public DbSet<AuditLogs> AuditLogs { get; set; }

        public DbSet<CenarioClinico> CenariosClinicos { get; set; }

        public DbSet<AvaliacaoCenario> AvaliacoesCenarios { get; set; }

    

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            

            // ==========================================
            // LOG DE AUDITORIA
            // ==========================================

            builder.Entity<AuditLogs>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                    .HasMaxLength(450);

                entity.Property(e => e.NomeProfissional)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Queixa)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Sintomas)
                    .HasMaxLength(2000);

                entity.Property(e => e.Discriminadores)
                    .HasMaxLength(3000);

                entity.Property(e => e.RegrasAplicadas)
                    .HasMaxLength(3000);

                entity.Property(e => e.Justificativa)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.Property(e => e.TipoOperacao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DataHora)
                    .IsRequired();

                entity.HasIndex(e => e.DataHora);

                entity.HasIndex(e => e.UserId);

                entity.HasIndex(e => e.CorRisco);
            });

            // ==========================================
            // CENÁRIOS
            // ==========================================

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

                // Cada participante responde cada cenário uma única vez
                entity.HasIndex(e => new
                {
                    e.CenarioClinicoId,
                    e.NomeProfissional
                })
                    .IsUnique();
            });

            // ==========================================
            // CENÁRIOS CLÍNICOS
            // ==========================================

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

            
        }
    }
}