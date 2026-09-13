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
        private readonly PlnService _pln;
        private readonly DatasetManchesterService _dataset;
        private readonly ManchesterRulesService _rules;
        private readonly MetricasService _metricas;

        public AvaliacaoController(
            ApplicationDbContext context,
            PlnService pln,
            DatasetManchesterService dataset,
            ManchesterRulesService rules,
            MetricasService metricas)
        {
            _context = context;
            _pln = pln;
            _dataset = dataset;
            _rules = rules;
            _metricas = metricas;
        }

        // =========================================================
        // LISTA DE CENÁRIOS
        // =========================================================

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

        // =========================================================
        // ABRIR CENÁRIO
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Responder(int id)
        {
            var cenario =
                await _context.CenariosClinicos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c =>
                        c.Id == id);

            if (cenario == null)
                return NotFound();

            return View(cenario);
        }

        // =========================================================
        // RESPONDER CENÁRIO
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Responder(
            int id,
            CorTriagem corProfissional)
        {
            var cenario =
                await _context.CenariosClinicos
                    .FirstOrDefaultAsync(c =>
                        c.Id == id);

            if (cenario == null)
                return NotFound();

            // -----------------------------------------------------
            // Montagem do texto clínico
            // -----------------------------------------------------

            var texto =
                $"{cenario.QueixaPrincipal} {cenario.Sintomas}";

            // -----------------------------------------------------
            // Extração dos discriminadores
            // -----------------------------------------------------

            var discriminadores =
                _pln.Extrair(
                    texto,
                    _dataset.Discriminadores);

            // -----------------------------------------------------
            // Avaliação dos sinais vitais
            // -----------------------------------------------------

            discriminadores.AddRange(
                _pln.AvaliarSinaisVitais(
                    cenario.FrequenciaCardiaca,
                    cenario.FrequenciaRespiratoria,
                    cenario.PressaoSistolica,
                    cenario.Saturacao,
                    cenario.Temperatura));

            // -----------------------------------------------------
            // Criação dos sinais vitais
            // -----------------------------------------------------

            var vitais =
                new SinaisVitais
                {
                    FrequenciaCardiaca =
                        cenario.FrequenciaCardiaca,

                    FrequenciaRespiratoria =
                        cenario.FrequenciaRespiratoria,

                    PressaoSistolica =
                        cenario.PressaoSistolica,

                    Saturacao =
                        cenario.Saturacao,

                    Temperatura =
                        cenario.Temperatura,

                    PacienteInconsciente =
                        cenario.PacienteInconsciente
                };

            // -----------------------------------------------------
            // Classificação do sistema
            // -----------------------------------------------------

            var resultado =
                _rules.Avaliar(
                    discriminadores,
                    vitais);

            // -----------------------------------------------------
            // Registro da avaliação
            // -----------------------------------------------------

            var avaliacao =
                new AvaliacaoCenario
                {
                    CenarioClinicoId =
                        cenario.Id,

                    NomeProfissional =
                        User.Identity?.Name ??
                        "Usuário",

                    CorProfissional =
                        corProfissional,

                    CorSistema =
                        resultado.Cor,

                    CorPadraoOuro =
                        cenario.CorPadraoOuro,

                    SistemaAcertou =
                        resultado.Cor ==
                        cenario.CorPadraoOuro,

                    ProfissionalAcertou =
                        corProfissional ==
                        cenario.CorPadraoOuro,

                    Subtriagem =
                        _metricas.EhSubtriagem(
                            resultado.Cor,
                            cenario.CorPadraoOuro),

                    Supertriagem =
                        _metricas.EhSupertriagem(
                            resultado.Cor,
                            cenario.CorPadraoOuro),

                    DiscriminadoresIdentificados =
                        string.Join(
                            ", ",
                            discriminadores
                                .Select(d => d.Nome)
                                .Distinct()),

                    RegrasAplicadas =
                        string.Join(
                            ", ",
                            resultado.RegrasAplicadas),

                    JustificativaSistema =
                        resultado.Justificativa,

                    DataHora =
                        DateTime.Now
                };

            _context.AvaliacoesCenarios
                .Add(avaliacao);

            await _context.SaveChangesAsync();

            return View(
                "Resultado",
                avaliacao);
        }

        // =========================================================
        // MÉTRICAS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Metricas()
        {
            var resultado =
                await _metricas.CalcularAsync();

            return View(resultado);
        }

        // =========================================================
        // QUESTIONÁRIO DE USABILIDADE
        // =========================================================

        [HttpGet]
        public IActionResult Questionario()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Questionario(
            RespostaQuestionario resposta)
        {
            resposta.NomeProfissional =
                User.Identity?.Name ??
                "Usuário";

            resposta.DataHora =
                DateTime.Now;

            _context.RespostasQuestionarios
                .Add(resposta);

            await _context.SaveChangesAsync();

            TempData["Mensagem"] =
                "Questionário registrado com sucesso.";

            return RedirectToAction(
                nameof(Index));
        }
    }
}