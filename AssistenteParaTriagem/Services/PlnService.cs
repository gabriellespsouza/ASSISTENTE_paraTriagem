using AssistenteParaTriagem.Models;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AssistenteParaTriagem.Services
{
    public class PlnService
    {
        // =========================================================
        // TERMOS QUE INDICAM NEGAÇÃO
        // =========================================================

        private static readonly string[] Negacoes =
        {
            "sem",
            "nega",
            "não",
            "nao",
            "ausente"
        };

        // =========================================================
        // CONFIGURAÇÕES DO MATCHING APROXIMADO
        // =========================================================

        private const int DistanciaMaximaTolerada = 1;

        private const int TamanhoMinimoParaFuzzy = 5;

        private readonly ILogger<PlnService> _logger;

        public PlnService(
            ILogger<PlnService> logger)
        {
            _logger = logger;
        }

        // =========================================================
        // NORMALIZAÇÃO
        // =========================================================

        private string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            texto = texto
                .ToLowerInvariant()
                .Trim();

            // Remove acentos
            var normalized =
                texto.Normalize(
                    NormalizationForm.FormD);

            var sb =
                new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c)
                    != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            texto = sb.ToString();

            // Mantém letras, números e espaços
            texto = Regex.Replace(
                texto,
                @"[^\w\s]",
                " ");

            // Remove espaços duplicados
            texto = Regex.Replace(
                texto,
                @"\s+",
                " ");

            return texto.Trim();
        }

        // =========================================================
        // EXTRAÇÃO DE DISCRIMINADORES
        // =========================================================

        public List<DiscriminadorManchester> Extrair(
            string texto,
            List<DiscriminadorManchester> baseDados)
        {
            var resultado =
                new List<DiscriminadorManchester>();

            if (string.IsNullOrWhiteSpace(texto))
            {
                _logger.LogDebug(
                    "PLN recebeu texto vazio.");

                return resultado;
            }

            if (baseDados == null ||
                !baseDados.Any())
            {
                _logger.LogWarning(
                    "PLN não recebeu nenhum discriminador no dataset.");

                return resultado;
            }

            var textoNormalizado =
                Normalizar(texto);

            var palavrasDoTexto =
                textoNormalizado
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries);

            _logger.LogDebug(
                "Texto original: {TextoOriginal}",
                texto);

            _logger.LogDebug(
                "Texto normalizado: {TextoNormalizado}",
                textoNormalizado);

            // =====================================================
            // Dicionário para impedir duplicações
            // =====================================================

            var encontrados =
                new Dictionary<
                    string,
                    (DiscriminadorManchester Discriminador,
                     string TermoCasado)>();

            // =====================================================
            // PERCORRE OS DISCRIMINADORES
            // =====================================================

            foreach (var discriminador in baseDados)
            {
                if (discriminador == null)
                    continue;

                var termos =
                    discriminador
                        .ObterTermosBusca()
                        .Where(t =>
                            !string.IsNullOrWhiteSpace(t))
                        .ToList();

                foreach (var termoOriginal in termos)
                {
                    var termo =
                        Normalizar(termoOriginal);

                    if (string.IsNullOrWhiteSpace(termo))
                        continue;

                    // =================================================
                    // VERIFICA SE O TERMO ESTÁ NEGADO
                    // =================================================

                    bool negado =
                        VerificarNegacao(
                            textoNormalizado,
                            termo);

                    if (negado)
                    {
                        _logger.LogDebug(
                            "Termo ignorado por negação: {Termo}",
                            termoOriginal);

                        continue;
                    }

                    // =================================================
                    // MATCH EXATO
                    // =================================================

                    bool casouExato =
                        Regex.IsMatch(
                            textoNormalizado,
                            $@"(?<!\w){Regex.Escape(termo)}(?!\w)",
                            RegexOptions.IgnoreCase);

                    // =================================================
                    // MATCH APROXIMADO
                    // =================================================

                    bool casouAproximado = false;

                    if (!casouExato &&
                        !termo.Contains(' ') &&
                        termo.Length >=
                            TamanhoMinimoParaFuzzy)
                    {
                        casouAproximado =
                            palavrasDoTexto.Any(
                                palavra =>
                                    Math.Abs(
                                        palavra.Length -
                                        termo.Length)
                                    <= DistanciaMaximaTolerada
                                    &&
                                    DistanciaLevenshtein(
                                        palavra,
                                        termo)
                                    <=
                                    DistanciaMaximaTolerada);
                    }

                    // =================================================
                    // SE NÃO CASOU, CONTINUA
                    // =================================================

                    if (!casouExato &&
                        !casouAproximado)
                    {
                        continue;
                    }

                    // =================================================
                    // DISCRIMINADOR ENCONTRADO
                    // =================================================

                    var chave =
                        Normalizar(
                            discriminador.PalavraChave);

                    if (string.IsNullOrWhiteSpace(chave))
                    {
                        chave =
                            discriminador.PalavraChave
                                .ToLowerInvariant();
                    }

                    if (!encontrados.ContainsKey(chave))
                    {
                        encontrados.Add(
                            chave,
                            (
                                discriminador,
                                termoOriginal
                            ));

                        _logger.LogInformation(
                            "Discriminador identificado: {Discriminador} | " +
                            "Termo encontrado: {Termo} | " +
                            "Cor: {Cor} | Gravidade: {Gravidade}",
                            discriminador.Nome,
                            termoOriginal,
                            discriminador.Cor,
                            discriminador.Gravidade);
                    }

                    // Já encontrou este discriminador.
                    break;
                }
            }

            // =====================================================
            // FILTRA TERMOS GENÉRICOS QUANDO EXISTE UM MAIS
            // ESPECÍFICO
            // =====================================================

            var filtrados =
                FiltrarPorEspecificidade(
                    encontrados);

            // =====================================================
            // ORDENA PELA MAIOR GRAVIDADE
            // =====================================================

            resultado =
                filtrados
                    .OrderByDescending(
                        d => d.Gravidade)
                    .ToList();

            _logger.LogInformation(
                "PLN extraiu {Quantidade} discriminador(es): {Discriminadores}",
                resultado.Count,
                resultado.Any()
                    ? string.Join(
                        ", ",
                        resultado.Select(
                            d =>
                                $"{d.Nome} ({d.Cor}, gravidade {d.Gravidade})"))
                    : "nenhum");

            return resultado;
        }

        // =========================================================
        // VERIFICAÇÃO DE NEGAÇÃO
        // =========================================================

        private bool VerificarNegacao(
            string texto,
            string termo)
        {
            foreach (var negacao in Negacoes)
            {
                var negacaoNormalizada =
                    Normalizar(negacao);

                // -------------------------------------------------
                // Termos compostos
                // -------------------------------------------------

                if (termo.Contains(' '))
                {
                    var padrao =
                        $@"\b{Regex.Escape(negacaoNormalizada)}\s+" +
                        $@"{Regex.Escape(termo)}\b";

                    if (Regex.IsMatch(
                        texto,
                        padrao,
                        RegexOptions.IgnoreCase))
                    {
                        return true;
                    }
                }
                else
                {
                    var padrao =
                        $@"\b{Regex.Escape(negacaoNormalizada)}\s+" +
                        $@"{Regex.Escape(termo)}\b";

                    if (Regex.IsMatch(
                        texto,
                        padrao,
                        RegexOptions.IgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // =========================================================
        // FILTRO DE ESPECIFICIDADE
        // =========================================================

        private List<DiscriminadorManchester>
            FiltrarPorEspecificidade(
                Dictionary<
                    string,
                    (DiscriminadorManchester Discriminador,
                     string TermoCasado)> encontrados)
        {
            var itens =
                encontrados.Values
                    .Select(v =>
                        (
                            v.Discriminador,
                            TermoNormalizado:
                                Normalizar(v.TermoCasado)
                        ))
                    .ToList();

            var chavesRemovidas =
                new HashSet<string>();

            foreach (var candidatoMenosEspecifico in itens)
            {
                foreach (var candidatoMaisEspecifico in itens)
                {
                    if (ReferenceEquals(
                        candidatoMenosEspecifico.Discriminador,
                        candidatoMaisEspecifico.Discriminador))
                    {
                        continue;
                    }

                    var termoCurto =
                        candidatoMenosEspecifico
                            .TermoNormalizado;

                    var termoLongo =
                        candidatoMaisEspecifico
                            .TermoNormalizado;

                    if (string.IsNullOrWhiteSpace(
                            termoCurto) ||
                        string.IsNullOrWhiteSpace(
                            termoLongo))
                    {
                        continue;
                    }

                    // =================================================
                    // Exemplo:
                    //
                    // "dispneia"
                    // "dispneia leve"
                    //
                    // Mantém "dispneia leve".
                    // =================================================

                    var estaContido =
                        termoLongo.Length >
                        termoCurto.Length
                        &&
                        Regex.IsMatch(
                            termoLongo,
                            $@"\b{Regex.Escape(termoCurto)}\b");

                    if (estaContido)
                    {
                        var chave =
                            Normalizar(
                                candidatoMenosEspecifico
                                    .Discriminador
                                    .PalavraChave);

                        chavesRemovidas.Add(chave);
                    }
                }
            }

            if (chavesRemovidas.Any())
            {
                _logger.LogDebug(
                    "Discriminadores removidos por especificidade: {Chaves}",
                    string.Join(
                        ", ",
                        chavesRemovidas));
            }

            return itens
                .Where(i =>
                    !chavesRemovidas.Contains(
                        Normalizar(
                            i.Discriminador
                                .PalavraChave)))
                .Select(i =>
                    i.Discriminador)
                .ToList();
        }

        // =========================================================
        // LEVENSHTEIN
        // =========================================================

        private static int DistanciaLevenshtein(
            string a,
            string b)
        {
            var custos =
                new int[b.Length + 1];

            for (int j = 0;
                 j <= b.Length;
                 j++)
            {
                custos[j] = j;
            }

            for (int i = 1;
                 i <= a.Length;
                 i++)
            {
                custos[0] = i;

                int anterior =
                    i - 1;

                for (int j = 1;
                     j <= b.Length;
                     j++)
                {
                    int atual =
                        custos[j];

                    int insercao =
                        custos[j] + 1;

                    int remocao =
                        custos[j - 1] + 1;

                    int substituicao =
                        anterior +
                        (a[i - 1] ==
                         b[j - 1]
                            ? 0
                            : 1);

                    custos[j] =
                        Math.Min(
                            Math.Min(
                                insercao,
                                remocao),
                            substituicao);

                    anterior = atual;
                }
            }

            return custos[b.Length];
        }

        // =========================================================
        // AVALIAÇÃO DOS SINAIS VITAIS
        // =========================================================

        public List<DiscriminadorManchester>
            AvaliarSinaisVitais(
                int? fc,
                int? fr,
                int? pas,
                int? spo2,
                double? temperatura)
        {
            _logger.LogDebug(
                "Sinais vitais informados: " +
                "FC={Fc}, FR={Fr}, PAS={Pas}, SpO2={Spo2}, Temp={Temp}",
                fc,
                fr,
                pas,
                spo2,
                temperatura);

            var lista =
                new List<DiscriminadorManchester>();

            // =====================================================
            // VERMELHO
            // =====================================================

            if (spo2.HasValue &&
                spo2.Value < 90)
            {
                lista.Add(
                    Novo(
                        "SpO₂ < 90%",
                        CorTriagem.Vermelho,
                        5,
                        "Hipoxemia grave"));
            }

            if (pas.HasValue &&
                pas.Value < 90)
            {
                lista.Add(
                    Novo(
                        "PAS < 90",
                        CorTriagem.Vermelho,
                        5,
                        "Hipotensão grave"));
            }

            if (fr.HasValue &&
                fr.Value > 35)
            {
                lista.Add(
                    Novo(
                        "FR > 35",
                        CorTriagem.Vermelho,
                        5,
                        "Taquipneia extrema"));
            }

            // =====================================================
            // LARANJA
            // =====================================================

            if (temperatura.HasValue &&
                temperatura.Value >= 39)
            {
                lista.Add(
                    Novo(
                        "Temperatura ≥ 39°C",
                        CorTriagem.Laranja,
                        4,
                        "Febre alta"));
            }

            if (fc.HasValue &&
                fc.Value > 130)
            {
                lista.Add(
                    Novo(
                        "FC > 130",
                        CorTriagem.Laranja,
                        4,
                        "Taquicardia importante"));
            }

            if (fr.HasValue &&
                fr.Value >= 30 &&
                fr.Value <= 35)
            {
                lista.Add(
                    Novo(
                        "FR 30-35",
                        CorTriagem.Laranja,
                        4,
                        "Taquipneia importante"));
            }

            // =====================================================
            // AMARELO
            // =====================================================

            if (fc.HasValue &&
                fc.Value > 100 &&
                fc.Value <= 130)
            {
                lista.Add(
                    Novo(
                        "FC 101-130",
                        CorTriagem.Amarelo,
                        3,
                        "Taquicardia moderada"));
            }

            if (fr.HasValue &&
                fr.Value >= 20 &&
                fr.Value < 30)
            {
                lista.Add(
                    Novo(
                        "FR 20-29",
                        CorTriagem.Amarelo,
                        3,
                        "Frequência respiratória aumentada"));
            }

            if (temperatura.HasValue &&
                temperatura.Value >= 37.5 &&
                temperatura.Value < 39)
            {
                lista.Add(
                    Novo(
                        "Febre moderada",
                        CorTriagem.Amarelo,
                        3,
                        "Temperatura elevada"));
            }

            // =====================================================
            // LOG
            // =====================================================

            if (lista.Any())
            {
                _logger.LogDebug(
                    "Discriminadores de sinais vitais encontrados: {Discriminadores}",
                    string.Join(
                        ", ",
                        lista.Select(
                            d =>
                                $"{d.Nome} ({d.Cor})")));
            }

            return lista
                .OrderByDescending(
                    x => x.Gravidade)
                .ToList();
        }

        // =========================================================
        // CRIA DISCRIMINADOR DE SINAL VITAL
        // =========================================================

        private DiscriminadorManchester Novo(
            string nome,
            CorTriagem cor,
            int gravidade,
            string justificativa)
        {
            return new DiscriminadorManchester
            {
                PalavraChave = nome,

                Nome = nome,

                Cor = cor,

                Gravidade = gravidade,

                Justificativa = justificativa
            };
        }
    }
}