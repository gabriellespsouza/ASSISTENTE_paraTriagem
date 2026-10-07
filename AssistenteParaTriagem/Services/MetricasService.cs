using AssistenteParaTriagem.Data;
using AssistenteParaTriagem.Models;
using Microsoft.EntityFrameworkCore;

namespace AssistenteParaTriagem.Services
{
    public class MetricasService
    {
        private readonly ApplicationDbContext _context;
        private readonly ValidacaoPlnService _validacaoPln;

        public MetricasService(
            ApplicationDbContext context,
            ValidacaoPlnService validacaoPln)
        {
            _context = context;
            _validacaoPln = validacaoPln;
        }

        // =========================================================
        // SUBTRIAGEM
        // =========================================================

        public bool EhSubtriagem(
            CorTriagem sistema,
            CorTriagem padraoOuro)
        {
            // Quanto maior o valor do enum, menor a prioridade.
            //
            // Vermelho = 0
            // Laranja  = 1
            // Amarelo  = 2
            // Verde    = 3
            // Azul     = 4
            //
            // Portanto:
            // sistema > padrão-ouro = sistema classificou
            // com menor prioridade.
            //
            // Exemplo:
            // Padrão-ouro = Laranja (1)
            // Sistema     = Amarelo (2)
            //
            // Isso caracteriza SUBTRIAGEM.

            return (int)sistema > (int)padraoOuro;
        }

        // =========================================================
        // SOBRETRIAGEM
        // =========================================================

        public bool EhSupertriagem(
            CorTriagem sistema,
            CorTriagem padraoOuro)
        {
            // Sistema classificou com prioridade maior
            // do que o padrão-ouro.

            return (int)sistema < (int)padraoOuro;
        }

        // =========================================================
        // CÁLCULO PRINCIPAL
        // =========================================================

        public async Task<ResultadoMetricas> CalcularAsync()
        {
            // =====================================================
            // IMPORTANTE:
            // Utilizamos SOMENTE as avaliações geradas pela
            // validação automática dos cenários simulados.
            //
            // Isso impede que avaliações antigas ou respostas
            // de profissionais alterem os resultados do TCC.
            // =====================================================

            var avaliacoes = await _context.AvaliacoesCenarios
                .AsNoTracking()
                .Where(a =>
                    a.NomeProfissional ==
                    ValidacaoAutomaticaService.Identificador)
                .OrderBy(a => a.CenarioClinicoId)
                .ToListAsync();

            // =====================================================
            // CARREGAR CENÁRIOS
            // =====================================================

            var cenarios = await _context.CenariosClinicos
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToListAsync();

            // =====================================================
            // VALIDAÇÃO ESPECÍFICA DO PLN
            // =====================================================

            var validacaoPln =
                _validacaoPln.Calcular(cenarios);

            var resultado = new ResultadoMetricas();

            // =====================================================
            // RESULTADOS DO PLN
            // =====================================================

            resultado.PlnTotalCenarios =
                validacaoPln.TotalCenarios;

            resultado.PlnVerdadeirosPositivos =
                validacaoPln.VerdadeirosPositivos;

            resultado.PlnFalsosPositivos =
                validacaoPln.FalsosPositivos;

            resultado.PlnFalsosNegativos =
                validacaoPln.FalsosNegativos;

            resultado.PlnPrecisao =
                validacaoPln.Precisao;

            resultado.PlnRecall =
                validacaoPln.Recall;

            resultado.PlnF1 =
                validacaoPln.F1;

            // =====================================================
            // NENHUMA AVALIAÇÃO DE CLASSIFICAÇÃO
            // =====================================================

            if (!avaliacoes.Any())
            {
                return resultado;
            }

            // =====================================================
            // TOTAL DE CASOS
            // =====================================================

            resultado.TotalCasos =
                avaliacoes.Count;

            // =====================================================
            // ACERTOS DO SISTEMA
            // =====================================================

            resultado.AcertosSistema =
                avaliacoes.Count(a =>
                    a.SistemaAcertou);

            // =====================================================
            // ACURÁCIA DO SISTEMA
            // =====================================================

            resultado.AcuraciaSistema =
                Percentual(
                    resultado.AcertosSistema,
                    resultado.TotalCasos);

            // =====================================================
            // SUBTRIAGEM
            // =====================================================

            resultado.UnderTriage =
                Percentual(
                    avaliacoes.Count(a =>
                        a.Subtriagem),
                    resultado.TotalCasos);

            // =====================================================
            // SOBRETRIAGEM
            // =====================================================

            resultado.OverTriage =
                Percentual(
                    avaliacoes.Count(a =>
                        a.Supertriagem),
                    resultado.TotalCasos);

            // =====================================================
            // PRECISÃO / RECALL / F1
            // =====================================================

            CalcularMetricasMulticlasse(
                avaliacoes,
                resultado);

            // =====================================================
            // KAPPA DE COHEN
            // =====================================================

            resultado.Kappa =
                CalcularKappa(
                    avaliacoes);

            return resultado;
        }

        // =========================================================
        // PERCENTUAL
        // =========================================================

        private static double Percentual(
            int valor,
            int total)
        {
            if (total == 0)
                return 0;

            return valor * 100.0 / total;
        }

        // =========================================================
        // PRECISÃO, RECALL E F1
        // =========================================================

        private static void CalcularMetricasMulticlasse(
            List<AvaliacaoCenario> avaliacoes,
            ResultadoMetricas resultado)
        {
            var classes =
                Enum.GetValues<CorTriagem>();

            var precisaoClasses =
                new List<double>();

            var recallClasses =
                new List<double>();

            var f1Classes =
                new List<double>();

            foreach (var classe in classes)
            {
                int verdadeirosPositivos = 0;
                int falsosPositivos = 0;
                int falsosNegativos = 0;

                foreach (var avaliacao in avaliacoes)
                {
                    bool sistemaEhClasse =
                        avaliacao.CorSistema == classe;

                    bool padraoEhClasse =
                        avaliacao.CorPadraoOuro == classe;

                    // -------------------------------------------------
                    // VERDADEIRO POSITIVO
                    // -------------------------------------------------

                    if (sistemaEhClasse &&
                        padraoEhClasse)
                    {
                        verdadeirosPositivos++;
                    }

                    // -------------------------------------------------
                    // FALSO POSITIVO
                    // -------------------------------------------------

                    else if (sistemaEhClasse &&
                             !padraoEhClasse)
                    {
                        falsosPositivos++;
                    }

                    // -------------------------------------------------
                    // FALSO NEGATIVO
                    // -------------------------------------------------

                    else if (!sistemaEhClasse &&
                             padraoEhClasse)
                    {
                        falsosNegativos++;
                    }
                }

                // =====================================================
                // PRECISÃO
                // =====================================================

                double precisao =
                    verdadeirosPositivos +
                    falsosPositivos == 0
                        ? 0
                        : verdadeirosPositivos * 100.0 /
                          (verdadeirosPositivos +
                           falsosPositivos);

                // =====================================================
                // RECALL
                // =====================================================

                double recall =
                    verdadeirosPositivos +
                    falsosNegativos == 0
                        ? 0
                        : verdadeirosPositivos * 100.0 /
                          (verdadeirosPositivos +
                           falsosNegativos);

                // =====================================================
                // F1
                // =====================================================

                double f1 = 0;

                if (precisao + recall > 0)
                {
                    f1 =
                        2 *
                        precisao *
                        recall /
                        (precisao + recall);
                }

                precisaoClasses.Add(precisao);
                recallClasses.Add(recall);
                f1Classes.Add(f1);
            }

            // =========================================================
            // MACRO MÉDIA
            // =========================================================

            resultado.Precisao =
                precisaoClasses.Any()
                    ? precisaoClasses.Average()
                    : 0;

            resultado.Recall =
                recallClasses.Any()
                    ? recallClasses.Average()
                    : 0;

            resultado.F1 =
                f1Classes.Any()
                    ? f1Classes.Average()
                    : 0;
        }

        // =========================================================
        // KAPPA DE COHEN MULTICLASSE
        // =========================================================

        private static double CalcularKappa(
            List<AvaliacaoCenario> avaliacoes)
        {
            if (avaliacoes.Count == 0)
                return 0;

            int total =
                avaliacoes.Count;

            // =====================================================
            // CONCORDÂNCIA OBSERVADA
            // =====================================================

            int concordantes =
                avaliacoes.Count(a =>
                    a.CorSistema ==
                    a.CorPadraoOuro);

            double concordanciaObservada =
                concordantes /
                (double)total;

            // =====================================================
            // DISTRIBUIÇÃO DAS CLASSES
            // =====================================================

            var classes =
                Enum.GetValues<CorTriagem>();

            double concordanciaEsperada = 0;

            foreach (var classe in classes)
            {
                int quantidadeSistema =
                    avaliacoes.Count(a =>
                        a.CorSistema == classe);

                int quantidadePadrao =
                    avaliacoes.Count(a =>
                        a.CorPadraoOuro == classe);

                double proporcaoSistema =
                    quantidadeSistema /
                    (double)total;

                double proporcaoPadrao =
                    quantidadePadrao /
                    (double)total;

                concordanciaEsperada +=
                    proporcaoSistema *
                    proporcaoPadrao;
            }

            // =====================================================
            // EVITA DIVISÃO POR ZERO
            // =====================================================

            if (Math.Abs(
                    1 -
                    concordanciaEsperada)
                < 0.000001)
            {
                return concordanciaObservada == 1
                    ? 1
                    : 0;
            }

            // =====================================================
            // FÓRMULA DO KAPPA
            // =====================================================

            return
                (concordanciaObservada -
                 concordanciaEsperada)
                /
                (1 -
                 concordanciaEsperada);
        }
    }
}