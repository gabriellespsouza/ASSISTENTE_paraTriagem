using AssistenteParaTriagem.Services;
using AssistenteParaTriagem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using AssistenteParaTriagem.Data;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// Banco de Dados
// ===============================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' não encontrada.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// ===============================
// Identity
// ===============================

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// ===============================
// MVC
// ===============================

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ===============================
// Serviços da aplicação
// ===============================

builder.Services.AddSingleton<DatasetManchesterService>();

builder.Services.AddScoped<PlnService>();

builder.Services.AddScoped<ManchesterRulesService>();

// ===============================

var app = builder.Build();

var cultura = new CultureInfo("pt-BR");

CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = new[] { cultura },
    SupportedUICultures = new[] { cultura }
});

// ===============================
// Pipeline
// ===============================

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

// IMPORTANTE

app.UseAuthentication();

app.UseAuthorization();

// ===============================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
