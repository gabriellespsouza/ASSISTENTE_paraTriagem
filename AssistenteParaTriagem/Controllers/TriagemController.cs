using AssistenteParaTriagem.Models;
using AssistenteParaTriagem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            // ==========================================
            // VALIDAÇÃO DA ENTRADA
            // ==========================================

            if (string.IsNullOrWhiteSpace(queixa))
            {
                ModelState.AddModelError(
                    "queixa",
                    "A queixa principal é obrigatória.");

                return View("Index");
            }

            // ==========================================
            // IDENTIFICAÇÃO DO USUÁRIO
            // ==========================================

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var nomeProfissional =
                User.Identity?.Name ?? "Usuário";

            // ==========================================
            // PROCESSAMENTO DO TEXTO
            // ==========================================

            var texto =
                $"{queixa} {sintomas}";

            var discriminadores =
                _pln.Extrair(
                    texto,
                    _dataset.Discriminadores);

            // ==========================================
            // ANÁLISE DOS SINAIS VITAIS
            // ==========================================

            discriminadores.AddRange(
                _pln.AvaliarSinaisVitais(
                    fc,
                    fr,
                    pas,
                    spo2,
                    temperatura));

            // ==========================================
            // OBJETO DE SINAIS VITAIS
            // ==========================================

            var vitais = new SinaisVitais
            {
                FrequenciaCardiaca = fc,
                FrequenciaRespiratoria = fr,
                PressaoSistolica = pas,
                Saturacao = spo2,
                Temperatura = temperatura,
                PacienteInconsciente =
                    pacienteInconsciente
            };

            // ==========================================
            // MOTOR DE REGRAS
            // ==========================================

            var resultado =
                _rules.Avaliar(
                    discriminadores,
                    vitais);

            // ==========================================
            // TRANSFORMAÇÃO DOS RESULTADOS
            // ==========================================

            var discriminadoresTexto =
                string.Join(
                    ", ",
                    discriminadores
                        .Select(x => x.Nome)
                        .Distinct());

            var regrasTexto =
                string.Join(
                    ", ",
                    resultado.RegrasAplicadas);

            // ==========================================
            // REGISTRO DA TRIAGEM
            // ==========================================

            var avaliacao =
                new AvaliacaoTriagem
                {
                    NomeProfissional =
                        nomeProfissional,

                    Queixa =
                        queixa,

                    Sintomas =
                        sintomas ?? string.Empty,

                    FrequenciaCardiaca =
                        fc,

                    FrequenciaRespiratoria =
                        fr,

                    PressaoSistolica =
                        pas,

                    Saturacao =
                        spo2,

                    Temperatura =
                        temperatura,

                    PacienteInconsciente =
                        pacienteInconsciente,

                    Discriminadores =
                        discriminadoresTexto,

                    RegrasAplicadas =
                        regrasTexto,

                    CorRisco =
                        resultado.Cor,

                    TempoMaximo =
                        resultado.TempoMaximo,

                    Justificativa =
                        resultado.Justificativa,

                    DataHora =
                        DateTime.Now
                };

            _context.Avaliacoes.Add(avaliacao);

            // ==========================================
            // LOG DE AUDITORIA
            // ==========================================
            //
            // Este registro guarda todo o caminho da decisão:
            //
            // Entrada
            //    ↓
            // PLN
            //    ↓
            // Discriminadores
            //    ↓
            // Motor de regras
            //    ↓
            // Classificação
            //    ↓
            // Justificativa
            //

            var auditLog =
                new AuditLogs
                {
                    UserId =
                        userId,

                    NomeProfissional =
                        nomeProfissional,

                    DataHora =
                        DateTime.Now,

                    Queixa =
                        queixa,

                    Sintomas =
                        sintomas ?? string.Empty,

                    FrequenciaCardiaca =
                        fc,

                    FrequenciaRespiratoria =
                        fr,

                    PressaoSistolica =
                        pas,

                    Saturacao =
                        spo2,

                    Temperatura =
                        temperatura,

                    PacienteInconsciente =
                        pacienteInconsciente,

                    Discriminadores =
                        discriminadoresTexto,

                    RegrasAplicadas =
                        regrasTexto,

                    CorRisco =
                        resultado.Cor,

                    TempoMaximo =
                        resultado.TempoMaximo,

                    Justificativa =
                        resultado.Justificativa,

                    TipoOperacao =
                        "Triagem",

                    DecisaoConcluida =
                        true
                };

            _context.AuditLogs.Add(auditLog);

            // ==========================================
            // SALVA TUDO NO BANCO
            // ==========================================

            _context.SaveChanges();

            // ==========================================
            // RETORNA RESULTADO
            // ==========================================

            return View(
                "Resultado",
                resultado);
        }
    }
}