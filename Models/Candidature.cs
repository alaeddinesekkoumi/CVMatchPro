namespace CVMatchPro.Models
{
    using System;

    public class Candidature
    {
        public int Id { get; set; }

        // 🔗 Lien avec le candidat
        public int CandidatId { get; set; }
        public Candidat? Candidat { get; set; }

        // 🔗 Lien avec l'offre
        public int OffreEmploiId { get; set; }
        public OffreEmploi? OffreEmploi { get; set; }

        // Infos supplémentaires
        public DateTime DatePostulation { get; set; } = DateTime.Now;
    }
}
