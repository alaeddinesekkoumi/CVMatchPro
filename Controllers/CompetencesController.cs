using CVMatchPro.Data;
using CVMatchPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CompetencesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CompetencesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var competences = await _context.Competences.ToListAsync();
        return View(competences);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Competence competence)
    {
        if (ModelState.IsValid)
        {
            _context.Competences.Add(competence);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(competence);
    }
}
