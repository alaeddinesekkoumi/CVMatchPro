using CVMatchPro.Data;
using CVMatchPro.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Candidat")]
public class CandidatController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CandidatController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Profil()
    {
        var userId = _userManager.GetUserId(User);
        var candidat = await _context.Candidats
            .Include(c => c.CVs) // inclure les CVs
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (candidat == null) return NotFound();

        return View(candidat);
    }

    // GET : Profil du candidat
    [HttpGet]
    public async Task<IActionResult> EditProfil()
    {
        var userId = _userManager.GetUserId(User);
        var candidat = await _context.Candidats.FirstOrDefaultAsync(c => c.UserId == userId);

        if (candidat == null) return NotFound();

        return View(candidat);
    }

    // POST : Mettre à jour le profil
    [HttpPost]
    public async Task<IActionResult> EditProfil(Candidat candidat, IFormFile? photo)
    {
        var userId = _userManager.GetUserId(User);
        var existingCandidat = await _context.Candidats.FirstOrDefaultAsync(c => c.UserId == userId);

        if (existingCandidat == null) return NotFound();

        // Upload de la photo si fournie
        if (photo != null && photo.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/candidats");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            existingCandidat.PhotoUrl = "/images/candidats/" + fileName;
        }

        // Mise à jour des infos
        existingCandidat.Nom = candidat.Nom;
        existingCandidat.Email = candidat.Email;
        existingCandidat.Telephone = candidat.Telephone;
        existingCandidat.Adresse = candidat.Adresse;

        _context.Update(existingCandidat);
        await _context.SaveChangesAsync();

        return RedirectToAction("Profil");
    }
}
