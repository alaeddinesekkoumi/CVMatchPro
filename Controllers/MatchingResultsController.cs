using CVMatchPro.Data;
using CVMatchPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class MatchingResultsController : Controller
{
    private readonly ApplicationDbContext _context;

    public MatchingResultsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var results = await _context.MatchingResults
            .Include(m => m.CV)
            .Include(m => m.OffreEmploi)
            .OrderByDescending(m => m.ScorePertinence) // 🔥 tri du plus pertinent au moins
            .ToListAsync();

        return View(results);
    }
}
