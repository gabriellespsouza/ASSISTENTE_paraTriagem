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

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Endpoint de diagnóstico: mostra quantos discriminadores foram
        /// carregados do manchester.csv e seus sinônimos, para conferir
        /// rapidamente se o léxico clínico controlado está ativo.
        /// Remova esta action antes de considerar o protótipo "final".
        /// </summary>
        public IActionResult Diagnostico()
        {
            return View(_dataset.Discriminadores);
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
            var texto = $"{queixa} {sintomas}";

            var discriminadores = _pln.Extrair(texto, _dataset.Discriminadores);

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

            // Log de auditoria completo: entradas, discriminadores,
            // regras ativadas e classificação final (seção 2.3 do
            // projeto de pesquisa).
            _context.Avaliacoes.Add(new AvaliacaoTriagem
            {
                NomeProfissional = User.Identity?.Name ?? "Usuário",

                Queixa = queixa,
                Sintomas = sintomas,

                FrequenciaCardiaca = fc,
                FrequenciaRespiratoria = fr,
                PressaoSistolica = pas,
                Saturacao = spo2,
                Temperatura = temperatura,

                PacienteInconsciente = pacienteInconsciente,

                Discriminadores = string.Join(", ",
                    discriminadores.Select(x => x.Nome)),

                RegrasAplicadas = string.Join(", ",
                    resultado.RegrasAplicadas),

                CorRisco = resultado.Cor,
                TempoMaximo = resultado.TempoMaximo,
                Justificativa = resultado.Justificativa,
                DataHora = DateTime.Now
            });

            _context.SaveChanges();

            return View("Resultado", resultado);
        }
    }
}