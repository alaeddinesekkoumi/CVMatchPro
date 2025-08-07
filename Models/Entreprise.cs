

namespace CVMatchPro.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;
    public class Entreprise
    {
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Email { get; set; }

        public string Secteur { get; set; }
        public string Adresse { get; set; }
        public string Pays { get; set; }

        // 🔗 Liaison avec Identity
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        // 🔗 Lié aux offres
        public ICollection<OffreEmploi> Offres { get; set; }
    }
}
