using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CVMatchPro.Models; // Assure-toi que ce namespace correspond à l'emplacement de tes classes

namespace CVMatchPro.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Déclarations des DbSet (tables)
        public DbSet<Candidat> Candidats { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<Entreprise> Entreprises { get; set; }
        public DbSet<OffreEmploi> OffresEmploi { get; set; }
        public DbSet<Competence> Competences { get; set; }
        public DbSet<MatchingResult> MatchingResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔗 Lien entre Candidat et IdentityUser
            modelBuilder.Entity<Candidat>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔗 Lien entre Entreprise et IdentityUser
            modelBuilder.Entity<Entreprise>()
                 .HasOne(e => e.User)
     .WithMany()
     .HasForeignKey(e => e.UserId)
     .OnDelete(DeleteBehavior.Restrict); // ✅ éviter la cascade


            // 🔗 Relation compétence <-> CV
            modelBuilder.Entity<Competence>()
                .HasOne(c => c.CV)
                .WithMany(cv => cv.Competences)
                .HasForeignKey(c => c.CVId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔗 Relation compétence <-> Offre
            modelBuilder.Entity<Competence>()
                .HasOne(c => c.OffreEmploi)
                .WithMany(o => o.CompetencesRequises)
                .HasForeignKey(c => c.OffreEmploiId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔗 Empêcher les doublons dans MatchingResult (CV + Offre)
            modelBuilder.Entity<MatchingResult>()
                .HasIndex(m => new { m.CVId, m.OffreEmploiId })
                .IsUnique();
        }

    }
}
