using System.ComponentModel.DataAnnotations;

namespace CVMatchPro.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Role { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Candidat
        public string? NomCandidat { get; set; }
        public string? PrenomCandidat { get; set; }

        // Entreprise
        public string? NomEntreprise { get; set; }
        public string? Pays { get; set; }
        public string? Adresse { get; set; }
        public string? Secteur { get; set; }
    }
}
