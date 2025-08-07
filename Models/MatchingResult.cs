namespace CVMatchPro.Models
{
    using System;

    public class MatchingResult
    {
        public int Id { get; set; }

        public int CVId { get; set; }
        public CV CV { get; set; }

        public int OffreEmploiId { get; set; }
        public OffreEmploi OffreEmploi { get; set; }

        public double ScorePertinence { get; set; }
        public DateTime DateMatching { get; set; } = DateTime.Now;
    }

}
