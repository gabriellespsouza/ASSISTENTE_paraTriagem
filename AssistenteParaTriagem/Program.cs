using AssistenteParaTriagem.Data;
using AssistenteParaTriagem.Models;
using AssistenteParaTriagem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' não encontrada.");

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(
    options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddSingleton<
    DatasetManchesterService>();

builder.Services.AddScoped<
    PlnService>();

builder.Services.AddScoped<
    ManchesterRulesService>();

builder.Services.AddScoped<
    ValidacaoPlnService>();

builder.Services.AddScoped<
    MetricasService>();

var app = builder.Build();

var cultura =
    new CultureInfo("pt-BR");

CultureInfo.DefaultThreadCurrentCulture =
    cultura;

CultureInfo.DefaultThreadCurrentUICulture =
    cultura;

app.UseRequestLocalization(
    new RequestLocalizationOptions
    {
        DefaultRequestCulture =
            new RequestCulture("pt-BR"),

        SupportedCultures =
            new[] { cultura },

        SupportedUICultures =
            new[] { cultura }
    });

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler(
        "/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<ApplicationDbContext>();

    await DbInitializer.InicializarAsync(context);
}

app.Run();