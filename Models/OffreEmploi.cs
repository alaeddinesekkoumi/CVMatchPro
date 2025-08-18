namespace CVMatchPro.Models
{
    using System;
    using System.Collections.Generic;

    public class OffreEmploi
    {
        public int Id { get; set; }
        public string? Titre { get; set; }
        public string? Description { get; set; }
        public string? Lieu { get; set; }
        public DateTime DatePublication { get; set; } = DateTime.Now;

        public int EntrepriseId { get; set; }
        public Entreprise? Entreprise { get; set; }

        public ICollection<Competence>? CompetencesRequises { get; set; }
        public ICollection<MatchingResult>? MatchingResults { get; set; }
    }

}
