using CVMatchPro.Data;
using CVMatchPro.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 📦 Connexion à la base de données
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 🔐 Identity avec rôles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 📄 MVC + Razor + services personnalisés
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Obligatoire pour les pages Identity
builder.Services.AddScoped<CustomLoginRedirect>();
builder.Services.AddScoped<MatchingService>(); // ✅ Enregistrement du service Matching

var app = builder.Build();

// ⚙️ Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Pour debug migration
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Authentification & autorisation (dans cet ordre)
app.UseAuthentication();
app.UseAuthorization();

// 📍 Routes principales
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "entreprises",
    pattern: "Entreprises/{action=Profil}/{id?}",
    defaults: new { controller = "Entreprises" });

// 🔐 Pages Razor (Login, Register, etc.)
app.MapRazorPages();

app.Run();
