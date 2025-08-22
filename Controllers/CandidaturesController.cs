using CVMatchPro.Data;
using CVMatchPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Candidat")] // seuls les candidats peuvent postuler
public class CandidaturesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CandidaturesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Postuler(int offreId)
    {
        var user = await _userManager.GetUserAsync(User);

        // Récupérer le candidat lié à cet utilisateur
        var candidat = _context.Candidats.FirstOrDefault(c => c.UserId == user.Id);
        if (candidat == null)
        {
            return Unauthorized("Aucun profil candidat trouvé.");
        }

        // Vérifier s’il n’a pas déjà postulé
        bool dejaPostule = _context.Candidatures
            .Any(c => c.CandidatId == candidat.Id && c.OffreEmploiId == offreId);

        if (dejaPostule)
        {
            TempData["Message"] = "⚠️ Vous avez déjà postulé à cette offre.";
            return RedirectToAction("Details", "OffreEmplois", new { id = offreId });
        }

        var candidature = new Candidature
        {
            CandidatId = candidat.Id,
            OffreEmploiId = offreId
        };

        _context.Candidatures.Add(candidature);
        await _context.SaveChangesAsync();

        TempData["Message"] = "✅ Votre candidature a été envoyée avec succès !";
        return RedirectToAction("Details", "OffreEmplois", new { id = offreId });
    }
}
