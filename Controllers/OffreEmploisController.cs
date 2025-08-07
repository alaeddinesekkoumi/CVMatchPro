using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CVMatchPro.Data;
using CVMatchPro.Models;
using CVMatchPro.Services;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Entreprise")]
public class OffreEmploisController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly MatchingService _matchingService;

    public OffreEmploisController(ApplicationDbContext context, UserManager<IdentityUser> userManager, MatchingService matchingService)
    {
        _context = context;
        _userManager = userManager;
        _matchingService = matchingService;
    }

    [HttpPost]
    public async Task<IActionResult> Creer([FromForm] OffreEmploi offre, [FromForm] string CompetencesText)
    {
        var userId = _userManager.GetUserId(User);
        var entreprise = await _context.Entreprises
            .Include(e => e.Offres)
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (entreprise == null)
            return Unauthorized();

        offre.DatePublication = DateTime.Now;
        offre.EntrepriseId = entreprise.Id;
        offre.CompetencesRequises = new List<Competence>();

        if (!string.IsNullOrWhiteSpace(CompetencesText))
        {
            var competences = CompetencesText.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(c => new Competence { Nom = c.Trim() })
                                             .ToList();
            offre.CompetencesRequises = competences;
        }

        _context.OffresEmploi.Add(offre);
        await _context.SaveChangesAsync();

        return RedirectToAction("Profil", "Entreprises");
    }

    [HttpPost]
    public async Task<IActionResult> Supprimer(int id)
    {
        var offre = await _context.OffresEmploi
            .Include(o => o.CompetencesRequises)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (offre == null) return NotFound();

        _context.Competences.RemoveRange(offre.CompetencesRequises);
        _context.OffresEmploi.Remove(offre);
        await _context.SaveChangesAsync();

        return RedirectToAction("Profil", "Entreprises");
    }

    [HttpPost]
    public async Task<IActionResult> Modifier(int id, [FromForm] string Titre, [FromForm] string Description, [FromForm] string Lieu, [FromForm] string CompetencesText)
    {
        var offre = await _context.OffresEmploi
            .Include(o => o.CompetencesRequises)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (offre == null) return NotFound();

        offre.Titre = Titre;
        offre.Description = Description;
        offre.Lieu = Lieu;

        _context.Competences.RemoveRange(offre.CompetencesRequises);

        if (!string.IsNullOrWhiteSpace(CompetencesText))
        {
            offre.CompetencesRequises = CompetencesText
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => new Competence { Nom = c.Trim(), OffreEmploiId = id })
                .ToList();
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Profil", "Entreprises");
    }

    // ✅ NOUVELLE ACTION POUR VOIR LES CANDIDATS PERTINENTS
    [HttpGet]
    public async Task<IActionResult> VoirCandidats(int offreId)
    {
        var offre = await _context.OffresEmploi
            .Include(o => o.CompetencesRequises)
            .FirstOrDefaultAsync(o => o.Id == offreId);

        if (offre == null) return NotFound();

        var tousLesCVs = await _context.CVs.Include(cv => cv.Candidat).ToListAsync();
        var competencesOffre = string.Join(", ", offre.CompetencesRequises.Select(c => c.Nom));

        var candidatsPertinents = new List<(CV cv, float score)>();

        foreach (var cv in tousLesCVs)
        {
            float score = _matchingService.EvaluerPertinence(competencesOffre, cv.Experience, cv.Formation);
            if (score > 0.5f) // Seuil à ajuster selon ton modèle
            {
                candidatsPertinents.Add((cv, score));
            }
        }

        ViewBag.TitreOffre = offre.Titre;
        ViewBag.CandidatsPertinents = candidatsPertinents.OrderByDescending(c => c.score).ToList();

        return View("CandidatsPourOffre");
    }
}
