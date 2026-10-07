using AssistenteParaTriagem.Data;
using AssistenteParaTriagem.Models;
using Microsoft.EntityFrameworkCore;

namespace AssistenteParaTriagem.Services
{
    public class ValidacaoAutomaticaService
    {
        public const string Identificador = "VALIDACAO-SIMULADA";

        private readonly ApplicationDbContext _context;
        private readonly PlnService _pln;
        private readonly ManchesterRulesService _rules;
        private readonly DatasetManchesterService _dataset;

        public ValidacaoAutomaticaService(
            ApplicationDbContext context,
            PlnService pln,
            ManchesterRulesService rules,
            DatasetManchesterService dataset)
        {
            _context = context;
            _pln = pln;
            _rules = rules;
            _dataset = dataset;
        }

        public async Task ExecutarAsync()
        {
            // =========================================================
            // 1. CARREGA OS CENÁRIOS
            // =========================================================

            var cenarios = await _context.CenariosClinicos
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (!cenarios.Any())
            {
                return;
            }

            // =========================================================
            // 2. CARREGA O DATASET DO MANCHESTER
            // =========================================================

            var baseDados = _dataset.Discriminadores;

            if (baseDados == null || !baseDados.Any())
            {
                throw new InvalidOperationException(
                    "Nenhum discriminador foi carregado do arquivo manchester.csv.");
            }

            // =========================================================
            // 3. REMOVE RESULTADOS ANTERIORES DA VALIDAÇÃO
            // =========================================================

            var resultadosAntigos =
                await _context.AvaliacoesCenarios
                    .Where(a =>
                        a.NomeProfissional ==
                        Identificador)
                    .ToListAsync();

            if (resultadosAntigos.Any())
            {
                _context.AvaliacoesCenarios
                    .RemoveRange(resultadosAntigos);

                await _context.SaveChangesAsync();
            }

            // =========================================================
            // 4. EXECUTA A VALIDAÇÃO DE CADA CENÁRIO
            // =========================================================

            foreach (var cenario in cenarios)
            {
                // =====================================================
                // 4.1 TEXTO PARA O PLN
                // =====================================================

                var texto =
                    $"{cenario.QueixaPrincipal} {cenario.Sintomas}";

                // =====================================================
                // 4.2 EXTRAÇÃO DOS DISCRIMINADORES PELO PLN
                // =====================================================

                var discriminadoresTexto =
                    _pln.Extrair(
                        texto,
                        baseDados);

                // =====================================================
                // 4.3 AVALIAÇÃO DOS SINAIS VITAIS
                // =====================================================

                var discriminadoresVitais =
                    _pln.AvaliarSinaisVitais(
                        cenario.FrequenciaCardiaca,
                        cenario.FrequenciaRespiratoria,
                        cenario.PressaoSistolica,
                        cenario.Saturacao,
                        cenario.Temperatura);

                // =====================================================
                // 4.4 JUNTA OS DISCRIMINADORES
                // =====================================================

                var todosDiscriminadores =
                    discriminadoresTexto
                        .Concat(discriminadoresVitais)
                        .GroupBy(d => d.Nome)
                        .Select(g => g.First())
                        .ToList();

                var nomesDiscriminadores =
                    todosDiscriminadores
                        .Select(d => d.Nome)
                        .Distinct()
                        .ToList();

                // =====================================================
                // 4.5 MONTA OS SINAIS VITAIS
                // =====================================================

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

                // =====================================================
                // 4.6 APLICA AS REGRAS DE TRIAGEM
                // =====================================================

                var resultado =
                    _rules.Avaliar(
                        todosDiscriminadores,
                        vitais);

                // =====================================================
                // 4.7 COMPARA COM O PADRÃO-OURO
                // =====================================================

                bool acertou =
                    resultado.Cor ==
                    cenario.CorPadraoOuro;

                bool subtriagem =
                    (int)resultado.Cor >
                    (int)cenario.CorPadraoOuro;

                bool supertriagem =
                    (int)resultado.Cor <
                    (int)cenario.CorPadraoOuro;

                // =====================================================
                // 4.8 SALVA O RESULTADO
                // =====================================================

                var avaliacao =
                    new AvaliacaoCenario
                    {
                        CenarioClinicoId =
                            cenario.Id,

                        // Identificador técnico da validação
                        NomeProfissional =
                            Identificador,

                      

                        // Resultado do sistema
                        CorSistema =
                            resultado.Cor,

                        // Padrão-ouro
                        CorPadraoOuro =
                            cenario.CorPadraoOuro,

                        SistemaAcertou =
                            acertou,

                        Subtriagem =
                            subtriagem,

                        Supertriagem =
                            supertriagem,

                        DiscriminadoresIdentificados =
                            string.Join(
                                ", ",
                                nomesDiscriminadores),

                        RegrasAplicadas =
                            string.Join(
                                " | ",
                                resultado.RegrasAplicadas),

                        JustificativaSistema =
                            resultado.Justificativa,

                        DataHora =
                            DateTime.Now
                    };

                _context.AvaliacoesCenarios
                    .Add(avaliacao);
            }

            // =========================================================
            // 5. SALVA OS 20 RESULTADOS
            // =========================================================

            await _context.SaveChangesAsync();
        }
    }
}