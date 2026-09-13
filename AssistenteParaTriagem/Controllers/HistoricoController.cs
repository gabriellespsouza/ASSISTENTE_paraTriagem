using AssistenteParaTriagem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenteParaTriagem.Controllers
{
    [Authorize]
    public class HistoricoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistoricoController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var historico =
                await _context.Avaliacoes
                    .OrderByDescending(a => a.DataHora)
                    .ToListAsync();

            return View(historico);
        }
    }
}