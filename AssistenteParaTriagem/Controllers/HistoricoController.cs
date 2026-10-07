using AssistenteParaTriagem.Models;
using AssistenteParaTriagem.Services;
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
        private readonly AnonimizadorService _anon;

        public HistoricoController(
            ApplicationDbContext context,
            AnonimizadorService anon)
        {
            _context = context;
            _anon = anon;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _anon.CodigoDoUsuario(User);

            var historico = await _context.AuditLogs
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.DataHora)
                .ToListAsync();

            return View(historico);
        }
    }
}