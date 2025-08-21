namespace CVMatchPro.Models
{
    using System;

    public class Notification
    {
        public int Id { get; set; }

        // Message de la notification (ex: "Le candidat X est intéressé par votre offre Y")
        public string? Message { get; set; }

        // Pour savoir si l’entreprise a déjà lu la notification
        public bool IsRead { get; set; } = false;

        // Date de création
        public DateTime DateCreated { get; set; } = DateTime.Now;

        // 🔗 Candidat intéressé
        public int CandidatId { get; set; }
        public Candidat? Candidat { get; set; }

        // 🔗 Offre concernée
        public int OffreEmploiId { get; set; }
        public OffreEmploi? OffreEmploi { get; set; }

        // 🔗 Entreprise qui reçoit la notification
        public int EntrepriseId { get; set; }
        public Entreprise? Entreprise { get; set; }
    }
}
