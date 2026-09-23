using AssistenteParaTriagem.Models;

namespace AssistenteParaTriagem.Services
{
    public class ValidacaoPlnService
    {
        private readonly PlnService _pln;
        private readonly DatasetManchesterService _dataset;

        public ValidacaoPlnService(
            PlnService pln,
            DatasetManchesterService dataset)
        {
            _pln = pln;
            _dataset = dataset;
        }

        public ResultadoValidacaoPln Calcular(
            IEnumerable<CenarioClinico> cenarios)
        {
            var resultado = new ResultadoValidacaoPln();

            int verdadeirosPositivos = 0;
            int falsosPositivos = 0;
            int falsosNegativos = 0;

            foreach (var cenario in cenarios)
            {
                // Só entra na validação se o cenário possuir
                // discriminadores esperados cadastrados.
                var esperadosNormalizados =
                    SepararDiscriminadores(
                        cenario.DiscriminadoresEsperados)
                    .Select(Normalizar)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToHashSet();

                // Sem discriminadores esperados não existe
                // referência para avaliar o PLN.
                if (esperadosNormalizados.Count == 0)
                {
                    continue;
                }

                resultado.TotalCenarios++;

                // Texto clínico utilizado pelo PLN.
                var texto =
                    $"{cenario.QueixaPrincipal} {cenario.Sintomas}";

                // Discriminadores identificados pelo PLN.
                var encontrados =
                    _pln.Extrair(
                        texto,
                        _dataset.Discriminadores);

                var encontradosNormalizados =
                    encontrados
                        .Select(d => d.Nome)
                        .Select(Normalizar)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .ToHashSet();

                // ==================================================
                // COMPARAÇÃO:
                // PLN x DISCRIMINADORES ESPERADOS
                // ==================================================

                var verdadeirosPositivosDoCenario =
                    encontradosNormalizados
                        .Intersect(esperadosNormalizados)
                        .Count();

                var falsosPositivosDoCenario =
                    encontradosNormalizados
                        .Except(esperadosNormalizados)
                        .Count();

                var falsosNegativosDoCenario =
                    esperadosNormalizados
                        .Except(encontradosNormalizados)
                        .Count();

                verdadeirosPositivos +=
                    verdadeirosPositivosDoCenario;

                falsosPositivos +=
                    falsosPositivosDoCenario;

                falsosNegativos +=
                    falsosNegativosDoCenario;
            }

            // ======================================================
            // RESULTADOS DA VALIDAÇÃO
            // ======================================================

            resultado.VerdadeirosPositivos =
                verdadeirosPositivos;

            resultado.FalsosPositivos =
                falsosPositivos;

            resultado.FalsosNegativos =
                falsosNegativos;

            // ======================================================
            // PRECISÃO
            // TP / (TP + FP)
            // ======================================================

            int denominadorPrecisao =
                verdadeirosPositivos +
                falsosPositivos;

            resultado.Precisao =
                denominadorPrecisao == 0
                    ? 0
                    : verdadeirosPositivos * 100.0 /
                      denominadorPrecisao;

            // ======================================================
            // RECALL / SENSIBILIDADE
            // TP / (TP + FN)
            // ======================================================

            int denominadorRecall =
                verdadeirosPositivos +
                falsosNegativos;

            resultado.Recall =
                denominadorRecall == 0
                    ? 0
                    : verdadeirosPositivos * 100.0 /
                      denominadorRecall;

            // ======================================================
            // F1-SCORE
            // ======================================================

            if (resultado.Precisao +
                resultado.Recall > 0)
            {
                resultado.F1 =
                    2 *
                    resultado.Precisao *
                    resultado.Recall /
                    (resultado.Precisao +
                     resultado.Recall);
            }
            else
            {
                resultado.F1 = 0;
            }

            return resultado;
        }

        private static IEnumerable<string>
            SepararDiscriminadores(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return Enumerable.Empty<string>();
            }

            return texto
                .Split(
                    new[]
                    {
                        ',',
                        ';',
                        '|'
                    },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim());
        }

        private static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            var normalizado =
                texto
                    .ToLowerInvariant()
                    .Normalize(
                        System.Text.NormalizationForm.FormD);

            var resultado =
                new System.Text.StringBuilder();

            foreach (var caractere in normalizado)
            {
                var categoria =
                    System.Globalization.CharUnicodeInfo
                        .GetUnicodeCategory(caractere);

                if (categoria !=
                    System.Globalization.UnicodeCategory
                        .NonSpacingMark)
                {
                    resultado.Append(caractere);
                }
            }

            return resultado
                .ToString()
                .Trim();
        }
    }
}