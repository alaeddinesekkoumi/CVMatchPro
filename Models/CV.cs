using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace CVMatchPro.Models
{
    public class CV
    {
        public int Id { get; set; }
        public string? FichierNom { get; set; }
        public string? Url { get; set; }

        // Données extraites
        public string? NomExtrait { get; set; }
        public string? EmailExtrait { get; set; }
        public string? Telephone { get; set; }
        public string? Experience { get; set; }
        public string? Formation { get; set; }
        public string? CentresInteret { get; set; }
        public string? CompetencesExtraites { get; set; }


        public DateTime DateDepot { get; set; } = DateTime.Now;

        public int CandidatId { get; set; }
        public Candidat? Candidat { get; set; }

        public ICollection<Competence> Competences { get; set; }
        public ICollection<MatchingResult> MatchingResults { get; set; }

        [NotMapped]
        public IFormFile FichierCV { get; set; }
    }
}