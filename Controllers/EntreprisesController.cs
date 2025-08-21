using CVMatchPro.Data;
using CVMatchPro.Models;
using CVMatchPro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Entreprise")]
public class EntreprisesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public EntreprisesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Profil()
    {
        var userId = _userManager.GetUserId(User);

        var entreprise = await _context.Entreprises
            .Include(e => e.Offres)
                .ThenInclude(o => o.CompetencesRequises)
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (entreprise == null) return NotFound();

        return View("Profil", entreprise);
    }
    // ✅ Créer une offre (GET)
    [HttpGet]
    public IActionResult Creer()
    {
        return PartialView("_FormulaireOffre", new OffreEmploi());
    }

    // ✅ Créer une offre (POST)
    [HttpPost]
    public async Task<IActionResult> Creer(OffreEmploi offre, string CompetencesText, [FromServices] MatchingService matchingService)
    {
        var userId = _userManager.GetUserId(User);
        var entreprise = await _context.Entreprises.FirstOrDefaultAsync(e => e.UserId == userId);
        if (entreprise == null) return NotFound();

        offre.DatePublication = DateTime.Now;
        offre.EntrepriseId = entreprise.Id;

        if (!string.IsNullOrWhiteSpace(CompetencesText))
        {
            offre.CompetencesRequises = CompetencesText
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => new Competence { Nom = c.Trim() })
                .ToList();
        }

        // ✅ Sauvegarde de l’offre
        _context.OffresEmploi.Add(offre);
        await _context.SaveChangesAsync();

        // ✅ Récupérer tous les CVs existants
        var cvs = await _context.CVs
            .Include(c => c.Candidat)
            .ToListAsync();

        // ✅ Générer les résultats de matching pour chaque CV
        foreach (var cv in cvs)
        {
            var score = matchingService.EvaluerPertinence(
                string.Join(", ", offre.CompetencesRequises.Select(c => c.Nom)),
                cv.Experience,
                cv.Formation
            );

            var result = new MatchingResult
            {
                OffreEmploiId = offre.Id,
                CVId = cv.Id,
                ScorePertinence = score
            };

            _context.MatchingResults.Add(result);
        }

        // ✅ Sauvegarder les résultats de matching
        await _context.SaveChangesAsync();

        return RedirectToAction("Profil");
    }



    // ✅ Voir les candidats correspondant à une offre
    public async Task<IActionResult> VoirCandidats(int id)
    {
        var offre = await _context.OffresEmploi
            .Include(o => o.MatchingResults)
                .ThenInclude(m => m.CV)
                    .ThenInclude(cv => cv.Candidat)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (offre == null) return NotFound();

        return View("CandidatsMatching", offre);
    }

    // ✅ Modifier une offre (GET)
    [HttpGet]
    public async Task<IActionResult> Modifier(int id)
    {
        var offre = await _context.OffresEmploi
            .Include(o => o.CompetencesRequises)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (offre == null) return NotFound();

        ViewBag.CompetencesText = string.Join(", ", offre.CompetencesRequises.Select(c => c.Nom));
        return View("Modifier", offre);
    }

    // ✅ Modifier une offre (POST)
    [HttpPost]
    public async Task<IActionResult> Modifier(int id, [Bind("Id,Titre,Description,Lieu")] OffreEmploi offreModifiee, string CompetencesText)
    {
        var offre = await _context.OffresEmploi
            .Include(o => o.CompetencesRequises)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (offre == null) return NotFound();

        // Mettre à jour les propriétés
        offre.Titre = offreModifiee.Titre;
        offre.Description = offreModifiee.Description;
        offre.Lieu = offreModifiee.Lieu;

        // Supprimer les anciennes compétences
        _context.Competences.RemoveRange(offre.CompetencesRequises);

        // Ajouter les nouvelles
        if (!string.IsNullOrWhiteSpace(CompetencesText))
        {
            offre.CompetencesRequises = CompetencesText
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => new Competence { Nom = c.Trim(), OffreEmploiId = offre.Id })
                .ToList();
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Profil");
    }

    // ✅ Supprimer une offre
    [HttpGet]
    public async Task<IActionResult> Supprimer(int id)
    {
        var offre = await _context.OffresEmploi
            .Include(o => o.CompetencesRequises)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (offre == null) return NotFound();

        _context.Competences.RemoveRange(offre.CompetencesRequises);
        _context.OffresEmploi.Remove(offre);
        await _context.SaveChangesAsync();

        return RedirectToAction("Profil");
    }

    [HttpGet]
    public async Task<IActionResult> EditProfil()
    {
        var userId = _userManager.GetUserId(User);
        var entreprise = await _context.Entreprises.FirstOrDefaultAsync(e => e.UserId == userId);

        if (entreprise == null) return NotFound();

        return View("Edit", entreprise);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfil(Entreprise entreprise, IFormFile? logo)
    {
        var userId = _userManager.GetUserId(User);
        var existingEntreprise = await _context.Entreprises.FirstOrDefaultAsync(e => e.UserId == userId);

        if (existingEntreprise == null) return NotFound();

        // 📌 Upload image si un fichier est fourni
        if (logo != null && logo.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(logo.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/entreprises");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await logo.CopyToAsync(stream);
            }

            // ✅ On stocke le chemin relatif pour que <img src=""> fonctionne
            existingEntreprise.LogoUrl = "/images/entreprises/" + fileName;
        }


        // ✅ Mettre à jour les infos
        existingEntreprise.Nom = entreprise.Nom;
        existingEntreprise.Email = entreprise.Email;
        existingEntreprise.Domaine = entreprise.Domaine;
        existingEntreprise.Secteur = entreprise.Secteur;
        existingEntreprise.Adresse = entreprise.Adresse;
        existingEntreprise.Ville = entreprise.Ville;
        existingEntreprise.Pays = entreprise.Pays;

        _context.Update(existingEntreprise);
        await _context.SaveChangesAsync();

        return RedirectToAction("Profil");
    }

}