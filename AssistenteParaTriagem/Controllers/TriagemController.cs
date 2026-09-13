using AssistenteParaTriagem.Models;
using AssistenteParaTriagem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteParaTriagem.Controllers
{
    [Authorize]
    public class TriagemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DatasetManchesterService _dataset;
        private readonly PlnService _pln;
        private readonly ManchesterRulesService _rules;

        public TriagemController(
            ApplicationDbContext context,
            DatasetManchesterService dataset,
            PlnService pln,
            ManchesterRulesService rules)
        {
            _context = context;
            _dataset = dataset;
            _pln = pln;
            _rules = rules;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Avaliar(
            string queixa,
            string sintomas,
            int? fc,
            int? fr,
            int? pas,
            int? spo2,
            double? temperatura,
            bool pacienteInconsciente)
        {
            if (string.IsNullOrWhiteSpace(queixa))
            {
                ModelState.AddModelError("queixa",
                    "A queixa principal é obrigatória.");

                return View("Index");
            }

            var texto = $"{queixa} {sintomas}";

            var discriminadores =
                _pln.Extrair(
                    texto,
                    _dataset.Discriminadores);

            discriminadores.AddRange(
                _pln.AvaliarSinaisVitais(
                    fc,
                    fr,
                    pas,
                    spo2,
                    temperatura));

            var vitais = new SinaisVitais
            {
                FrequenciaCardiaca = fc,
                FrequenciaRespiratoria = fr,
                PressaoSistolica = pas,
                Saturacao = spo2,
                Temperatura = temperatura,
                PacienteInconsciente = pacienteInconsciente
            };

            var resultado = _rules.Avaliar(
                discriminadores,
                vitais);

            var avaliacao = new AvaliacaoTriagem
            {
                NomeProfissional =
                    User.Identity?.Name ?? "Usuário",

                Queixa = queixa,
                Sintomas = sintomas ?? string.Empty,

                FrequenciaCardiaca = fc,
                FrequenciaRespiratoria = fr,
                PressaoSistolica = pas,
                Saturacao = spo2,
                Temperatura = temperatura,

                PacienteInconsciente =
                    pacienteInconsciente,

                Discriminadores =
                    string.Join(
                        ", ",
                        discriminadores.Select(x => x.Nome)),

                RegrasAplicadas =
                    string.Join(
                        ", ",
                        resultado.RegrasAplicadas),

                CorRisco = resultado.Cor,

                TempoMaximo =
                    resultado.TempoMaximo,

                Justificativa =
                    resultado.Justificativa,

                DataHora = DateTime.Now
            };

            _context.Avaliacoes.Add(avaliacao);

            _context.SaveChanges();

            return View(
                "Resultado",
                resultado);
        }
    }
}