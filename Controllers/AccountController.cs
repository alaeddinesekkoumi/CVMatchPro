using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CVMatchPro.Models;
using CVMatchPro.Data;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _roleManager = roleManager;
    }

    // ============ REGISTER ============
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            var role = new IdentityRole(model.Role);
            await _roleManager.CreateAsync(role);
        }

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, model.Role);

            if (model.Role == "Candidat")
            {
                var candidat = new Candidat
                {
                    Nom = model.NomCandidat + " " + model.PrenomCandidat,
                    Email = model.Email,
                    UserId = user.Id
                };
                _context.Candidats.Add(candidat);
            }
            else if (model.Role == "Entreprise")
            {
                var entreprise = new Entreprise
                {
                    Nom = model.NomEntreprise,
                    Email = model.Email,
                    Adresse = model.Adresse,
                    Secteur = model.Secteur,
                    Pays = model.Pays,
                    UserId = user.Id
                };
                _context.Entreprises.Add(entreprise);
            }

            await _context.SaveChangesAsync();
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }

    // ============ LOGIN ============
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Tentative de connexion invalide.");
        return View(model);
    }

    // ============ LOGOUT ============
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
