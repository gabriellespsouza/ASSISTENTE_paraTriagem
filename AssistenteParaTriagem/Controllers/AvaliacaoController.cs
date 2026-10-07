using AssistenteParaTriagem.Models;
using AssistenteParaTriagem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenteParaTriagem.Controllers
{
    [Authorize]
    public class AvaliacaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MetricasService _metricas;
        private readonly ValidacaoAutomaticaService _validacaoAutomatica;

        public AvaliacaoController(
            ApplicationDbContext context,
            MetricasService metricas,
            ValidacaoAutomaticaService validacaoAutomatica)
        {
            _context = context;
            _metricas = metricas;
            _validacaoAutomatica = validacaoAutomatica;
        }

        // Lista dos cenários simulados + botão de validação
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cenarios =
                await _context.CenariosClinicos
                    .AsNoTracking()
                    .OrderBy(c => c.Id)
                    .ToListAsync();

            return View(cenarios);
        }

        // Resultados da validação
        [HttpGet]
        public async Task<IActionResult> Metricas()
        {
            var resultado =
                await _metricas.CalcularAsync();

            return View(resultado);
        }

        // Executa a validação automática dos cenários
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecutarValidacaoAutomatica()
        {
            await _validacaoAutomatica.ExecutarAsync();

            TempData["Sucesso"] =
                "Validação automática dos cenários simulados executada com sucesso.";

            return RedirectToAction(nameof(Metricas));
        }
    }
}