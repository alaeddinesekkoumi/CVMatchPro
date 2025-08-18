using CVMatchPro;
using CVMatchPro.Data;
using CVMatchPro.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ML; // ✅ PredictionEnginePool
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

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

// ⚙️ Configuration du cookie d’authentification
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // ✅ redirection vers Login si non connecté
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 📄 MVC + Razor + services personnalisés
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<CustomLoginRedirect>();

// ✅ Injection du moteur ML.NET (PredictionEnginePool)
builder.Services.AddPredictionEnginePool<MLModel.ModelInput, MLModel.ModelOutput>()
    .FromFile(
        modelName: "CVMatcher",
filePath: Path.Combine(env.ContentRootPath, "MLModel.zip"),
        watchForChanges: true
    );



// ✅ Enregistrement du service Matching
builder.Services.AddScoped<MatchingService>();

var app = builder.Build();

// ⚙️ Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Authentification & autorisation (ordre correct)
app.UseAuthentication();
app.UseAuthorization();

// 📍 Routes principales
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// 📍 Route entreprise (optionnelle, si tu veux accès direct via /Entreprises/Profil)
app.MapControllerRoute(
    name: "entreprises",
    pattern: "Entreprises/{action=Profil}/{id?}",
    defaults: new { controller = "Entreprises" });

// 🔐 Pages Razor (Identity)
app.MapRazorPages();

// 🚀 Test ML.NET
MLModel.Train("MLModel.zip"); // Entraîne et sauvegarde le modèle

var sample = new MLModel.ModelInput
{
    Competences = ".NET; SQL; ASP.NET",
    Experience = "2 ans développeur web",
    Formation = "Master Informatique"
};

var prediction = MLModel.Predict(sample);
Console.WriteLine($"Score prédit : {prediction.PredictedScore}");


app.Run();
