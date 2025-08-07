using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CVMatchPro.Data;
using CVMatchPro.Models;
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
            .Include(c => c.CVs)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (candidat == null) return NotFound();

        return View(candidat);
    }
}
