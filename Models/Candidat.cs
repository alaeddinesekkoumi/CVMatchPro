namespace CVMatchPro.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;

    public class Candidat
    {
        public int Id { get; set; }

        [Required]
        public string? Nom { get; set; }

        [Required]
        public string? Email { get; set; }

        // Infos supplémentaires pour le profil
        public string? Telephone { get; set; }
        public string? Adresse { get; set; }
        public string? PhotoUrl { get; set; } // chemin vers la photo du candidat

        // 🔗 Liaison avec Identity
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        // 🔗 CVs
        public ICollection<CV>? CVs { get; set; }
    }
}
