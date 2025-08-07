using Microsoft.AspNetCore.Mvc;
using CVMatchPro.Data;
using CVMatchPro.Models;
using CVMatchPro.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Threading.Tasks;

[Authorize(Roles = "Candidat")]
public class CVsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly CVParserService _parser;

    public CVsController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _env = env;
        _userManager = userManager;
        _parser = new CVParserService(); // extraction texte seulement
    }

    // 📥 Page Upload
    public IActionResult Upload()
    {
        return View();
    }

    // 📤 Traitement Upload
    [HttpPost]
    public async Task<IActionResult> Upload(CV model)
    {
        if (model.FichierCV != null && model.FichierCV.Length > 0)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Path.GetFileNameWithoutExtension(model.FichierCV.FileName)
                         + "_" + Guid.NewGuid().ToString("N").Substring(0, 8)
                         + Path.GetExtension(model.FichierCV.FileName);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.FichierCV.CopyToAsync(stream);
            }

            // 1️⃣ Extraire texte du CV avec CVParserService
            var texte = _parser.ExtractText(filePath);

            // 2️⃣ Extraire données structurées via microservice Python (Flask)
            var iaParser = new IAParserService();
            var extrait = await iaParser.ExtractDataFromTextAsync(texte);

            // 3️⃣ Attribution des données extraites
            model.NomExtrait = extrait.NomExtrait;
            model.EmailExtrait = extrait.EmailExtrait;
            model.Telephone = extrait.Telephone;
            model.Experience = extrait.Experience;
            model.Formation = extrait.Formation;
            model.CompetencesExtraites = extrait.CompetencesExtraites;
            model.CentresInteret = extrait.CentresInteret;

            // 📁 Fichier
            model.FichierNom = fileName;
            model.Url = "/uploads/" + fileName;
            model.DateDepot = DateTime.Now;

            // 👤 Lier au candidat connecté
            var userId = _userManager.GetUserId(User);
            var candidat = await _context.Candidats.FirstOrDefaultAsync(c => c.UserId == userId);

            if (candidat != null)
            {
                model.CandidatId = candidat.Id;
                _context.CVs.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
        }

        // 🔁 En cas d’échec
        ModelState.AddModelError("", "Échec lors du téléversement ou du traitement du CV.");
        return View(model);
    }

    // 🗑️ Suppression
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var cv = await _context.CVs.FindAsync(id);
        if (cv != null)
        {
            var filePath = Path.Combine(_env.WebRootPath, "uploads", cv.FichierNom);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _context.CVs.Remove(cv);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    // 🧾 Profil du Candidat (liste de ses CVs)
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var candidat = await _context.Candidats
                                     .Include(c => c.CVs)
                                     .FirstOrDefaultAsync(c => c.UserId == userId);

        if (candidat == null) return NotFound();

        return View("~/Views/Candidat/Profil.cshtml", candidat);
    }
}
