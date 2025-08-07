using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CVMatchPro.Services
{
    public class CustomLoginRedirect
    {
        private readonly UserManager<IdentityUser> _userManager;

        public CustomLoginRedirect(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GetRedirectUrl(IdentityUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Candidat"))
                return "/Candidat/Profil";

            if (roles.Contains("Entreprise"))
                return "/Entreprise/Profil";

            return "/"; // Par défaut
        }
    }
}
