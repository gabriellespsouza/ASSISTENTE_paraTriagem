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

            var verdadeirosPositivos = 0;
            var falsosPositivos = 0;
            var falsosNegativos = 0;

            foreach (var cenario in cenarios)
            {
                resultado.TotalCenarios++;

                var texto =
                    $"{cenario.QueixaPrincipal} {cenario.Sintomas}";

                // ============================================
                // DISCRIMINADORES ENCONTRADOS PELO PLN
                // ============================================

                var encontrados =
                    _pln.Extrair(
                        texto,
                        _dataset.Discriminadores);

                var encontradosNormalizados =
                    encontrados
                        .Select(d => Normalizar(d.Nome))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .ToHashSet();

                // ============================================
                // DISCRIMINADORES ESPERADOS
                // ============================================

                var esperadosNormalizados =
                    SepararDiscriminadores(
                        cenario.DiscriminadoresEsperados)
                    .Select(Normalizar)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToHashSet();

                // ============================================
                // VERDADEIROS POSITIVOS
                // ============================================

                verdadeirosPositivos +=
                    encontradosNormalizados
                        .Intersect(esperadosNormalizados)
                        .Count();

                // ============================================
                // FALSOS POSITIVOS
                // ============================================

                falsosPositivos +=
                    encontradosNormalizados
                        .Except(esperadosNormalizados)
                        .Count();

                // ============================================
                // FALSOS NEGATIVOS
                // ============================================

                falsosNegativos +=
                    esperadosNormalizados
                        .Except(encontradosNormalizados)
                        .Count();
            }

            resultado.VerdadeirosPositivos =
                verdadeirosPositivos;

            resultado.FalsosPositivos =
                falsosPositivos;

            resultado.FalsosNegativos =
                falsosNegativos;

            // ============================================
            // PRECISÃO
            // ============================================

            var denominadorPrecisao =
                verdadeirosPositivos +
                falsosPositivos;

            resultado.Precisao =
                denominadorPrecisao == 0
                    ? 0
                    : verdadeirosPositivos * 100.0 /
                      denominadorPrecisao;

            // ============================================
            // RECALL
            // ============================================

            var denominadorRecall =
                verdadeirosPositivos +
                falsosNegativos;

            resultado.Recall =
                denominadorRecall == 0
                    ? 0
                    : verdadeirosPositivos * 100.0 /
                      denominadorRecall;

            // ============================================
            // F1
            // ============================================

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

            return resultado;
        }

        private static IEnumerable<string>
            SepararDiscriminadores(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return Enumerable.Empty<string>();

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

        private static string Normalizar(
            string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var normalizado =
                texto
                    .ToLowerInvariant()
                    .Normalize(
                        System.Text.NormalizationForm.FormD);

            var resultado = new System.Text.StringBuilder();

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
                .ToLowerInvariant()
                .Trim();
        }
    }
}