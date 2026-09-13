using AssistenteParaTriagem.Models;
using Microsoft.EntityFrameworkCore;

namespace AssistenteParaTriagem.Services
{
    public class MetricasService
    {
        private readonly ApplicationDbContext _context;

        public MetricasService(
            ApplicationDbContext context)
        {
            _context = context;
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
            // Portanto, sistema > padrão-ouro significa
            // que o sistema classificou com menor prioridade.

            return (int)sistema > (int)padraoOuro;
        }

        // =========================================================
        // SOBRETRIAGEM
        // =========================================================

        public bool EhSupertriagem(
            CorTriagem sistema,
            CorTriagem padraoOuro)
        {
            return (int)sistema < (int)padraoOuro;
        }

        // =========================================================
        // CÁLCULO PRINCIPAL
        // =========================================================

        public async Task<ResultadoMetricas> CalcularAsync()
        {
            var avaliacoes =
                await _context.AvaliacoesCenarios
                    .AsNoTracking()
                    .ToListAsync();

            var questionarios =
                await _context.RespostasQuestionarios
                    .AsNoTracking()
                    .ToListAsync();

            var resultado =
                new ResultadoMetricas();

            // -----------------------------------------------------
            // Nenhuma avaliação
            // -----------------------------------------------------

            if (!avaliacoes.Any())
                return resultado;

            // -----------------------------------------------------
            // TOTAL
            // -----------------------------------------------------

            resultado.TotalCasos =
                avaliacoes.Count;

            // -----------------------------------------------------
            // ACERTOS DO SISTEMA
            // -----------------------------------------------------

            resultado.AcertosSistema =
                avaliacoes.Count(a =>
                    a.SistemaAcertou);

            // -----------------------------------------------------
            // ACERTOS DOS PROFISSIONAIS
            // -----------------------------------------------------

            resultado.AcertosProfissionais =
                avaliacoes.Count(a =>
                    a.ProfissionalAcertou);

            // -----------------------------------------------------
            // ACURÁCIA DO SISTEMA
            // -----------------------------------------------------

            resultado.AcuraciaSistema =
                Percentual(
                    resultado.AcertosSistema,
                    resultado.TotalCasos);

            // -----------------------------------------------------
            // ACURÁCIA DOS PROFISSIONAIS
            // -----------------------------------------------------

            resultado.AcuraciaProfissionais =
                Percentual(
                    resultado.AcertosProfissionais,
                    resultado.TotalCasos);

            // -----------------------------------------------------
            // SUBTRIAGEM
            // -----------------------------------------------------

            resultado.UnderTriage =
                Percentual(
                    avaliacoes.Count(a =>
                        a.Subtriagem),
                    resultado.TotalCasos);

            // -----------------------------------------------------
            // SOBRETRIAGEM
            // -----------------------------------------------------

            resultado.OverTriage =
                Percentual(
                    avaliacoes.Count(a =>
                        a.Supertriagem),
                    resultado.TotalCasos);

            // -----------------------------------------------------
            // PRECISÃO / RECALL / F1
            // -----------------------------------------------------

            CalcularMetricasMulticlasse(
                avaliacoes,
                resultado);

            // -----------------------------------------------------
            // KAPPA
            // -----------------------------------------------------

            resultado.Kappa =
                CalcularKappa(
                    avaliacoes);

            // -----------------------------------------------------
            // CONCORDÂNCIA PROFISSIONAL × SISTEMA
            // -----------------------------------------------------

            resultado.ConcordanciaProfissionalSistema =
                Percentual(
                    avaliacoes.Count(a =>
                        a.CorProfissional ==
                        a.CorSistema),
                    resultado.TotalCasos);

            // -----------------------------------------------------
            // QUESTIONÁRIO DE USABILIDADE
            // -----------------------------------------------------

            if (questionarios.Any())
            {
                resultado.MediaFacilidade =
                    questionarios.Average(
                        q => q.FacilidadeUso);

                resultado.MediaClareza =
                    questionarios.Average(
                        q => q.ClarezaRecomendacao);

                resultado.MediaUtilidade =
                    questionarios.Average(
                        q => q.Utilidade);

                resultado.MediaConfianca =
                    questionarios.Average(
                        q => q.Confianca);

                resultado.MediaRecomendacao =
                    questionarios.Average(
                        q => q.RecomendariaUso);
            }

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

                    if (sistemaEhClasse &&
                        padraoEhClasse)
                    {
                        verdadeirosPositivos++;
                    }
                    else if (sistemaEhClasse &&
                             !padraoEhClasse)
                    {
                        falsosPositivos++;
                    }
                    else if (!sistemaEhClasse &&
                             padraoEhClasse)
                    {
                        falsosNegativos++;
                    }
                }

                double precisao =
                    verdadeirosPositivos +
                    falsosPositivos == 0
                        ? 0
                        : verdadeirosPositivos * 100.0 /
                          (verdadeirosPositivos +
                           falsosPositivos);

                double recall =
                    verdadeirosPositivos +
                    falsosNegativos == 0
                        ? 0
                        : verdadeirosPositivos * 100.0 /
                          (verdadeirosPositivos +
                           falsosNegativos);

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

            // -----------------------------------------------------
            // Macro média
            // -----------------------------------------------------

            resultado.Precisao =
                precisaoClasses.Average();

            resultado.Recall =
                recallClasses.Average();

            resultado.F1 =
                f1Classes.Average();
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

            // -----------------------------------------------------
            // Concordância observada
            // -----------------------------------------------------

            int concordantes =
                avaliacoes.Count(a =>
                    a.CorSistema ==
                    a.CorPadraoOuro);

            double concordanciaObservada =
                concordantes /
                (double)total;

            // -----------------------------------------------------
            // Distribuição do sistema
            // -----------------------------------------------------

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

            // -----------------------------------------------------
            // Evita divisão por zero
            // -----------------------------------------------------

            if (Math.Abs(
                    1 -
                    concordanciaEsperada)
                < 0.000001)
            {
                return concordanciaObservada == 1
                    ? 1
                    : 0;
            }

            // -----------------------------------------------------
            // Fórmula do Kappa
            // -----------------------------------------------------

            return
                (concordanciaObservada -
                 concordanciaEsperada)
                /
                (1 -
                 concordanciaEsperada);
        }
    }
}