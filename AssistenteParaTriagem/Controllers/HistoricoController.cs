using AssistenteParaTriagem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AssistenteParaTriagem.Controllers
{
    [Authorize]
    public class HistoricoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistoricoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var historico = await _context.AuditLogs
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.DataHora)
                .ToListAsync();

            return View(historico);
        }
    }
}