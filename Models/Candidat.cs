

namespace CVMatchPro.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;
    public class Candidat
    {
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Email { get; set; }

        // 🔗 Liaison avec Identity
        public string UserId { get; set; }
        public IdentityUser? User { get; set; }

        public ICollection<CV> CVs { get; set; }
    }
}
