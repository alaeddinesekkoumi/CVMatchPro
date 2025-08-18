namespace CVMatchPro.Models
{
    public class Competence
    {
        public int Id { get; set; }
        public string? Nom { get; set; }

        // Une compétence peut appartenir à un CV ou à une Offre
        public int? CVId { get; set; }
        public CV? CV { get; set; }

        public int? OffreEmploiId { get; set; }
        public OffreEmploi? OffreEmploi { get; set; }
    }

}
