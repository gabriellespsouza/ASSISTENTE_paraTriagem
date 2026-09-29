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
        // CONTROLE DE RESPOSTAS REPETIDAS
        // =========================================================

        private string NomeParticipante()
        {
            return User.Identity?.Name ?? "Usuário";
        }

        private async Task<bool> JaRespondeuAsync(
            int cenarioId)
        {
            var nome = NomeParticipante();

            return await _context.AvaliacoesCenarios
                .AsNoTracking()
                .AnyAsync(a =>
                    a.CenarioClinicoId == cenarioId &&
                    a.NomeProfissional == nome);
        }

        private IActionResult RedirecionarJaRespondido()
        {
            TempData["Aviso"] =
                "Você já respondeu este cenário. " +
                "Cada cenário pode ser respondido uma única vez.";

            return RedirectToAction(nameof(Index));
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

            // Cenários que este participante já respondeu
            var nome = NomeParticipante();

            var respondidos =
                await _context.AvaliacoesCenarios
                    .AsNoTracking()
                    .Where(a => a.NomeProfissional == nome)
                    .Select(a => a.CenarioClinicoId)
                    .ToListAsync();

            ViewBag.Respondidos = respondidos.ToHashSet();

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

            if (await JaRespondeuAsync(id))
                return RedirecionarJaRespondido();

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
            // Cada participante responde cada cenário uma única vez
            // -----------------------------------------------------

            if (await JaRespondeuAsync(id))
                return RedirecionarJaRespondido();

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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Duas requisições simultâneas (ex.: clique duplo):
                // o índice único do banco barra a segunda.
                _context.Entry(avaliacao).State =
                    EntityState.Detached;

                if (await JaRespondeuAsync(id))
                    return RedirecionarJaRespondido();

                throw;
            }

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