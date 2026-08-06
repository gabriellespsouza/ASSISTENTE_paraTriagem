using AssistenteParaTriagem.Models;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AssistenteParaTriagem.Services
{
    public class PlnService
    {
        private static readonly string[] Negacoes =
        {
            "sem",
            "nega",
            "não",
            "nao",
            "ausente",
            "sem sinais de"
        };

        // Distância máxima de edição (Levenshtein) tolerada para
        // considerar uma palavra do texto como uma grafia incorreta
        // de um termo do léxico clínico controlado.
        private const int DistanciaMaximaTolerada = 1;

        // Tamanho mínimo do termo para aplicar tolerância a erros de
        // digitação — evita falsos positivos em palavras curtas.
        private const int TamanhoMinimoParaFuzzy = 5;

        private readonly ILogger<PlnService> _logger;

        public PlnService(ILogger<PlnService> logger)
        {
            _logger = logger;
        }

        private string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            texto = texto.ToLower().Trim();

            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            texto = sb.ToString();

            // Remove pontuação
            texto = Regex.Replace(texto, @"[^\w\s]", " ");

            // Remove espaços duplicados
            texto = Regex.Replace(texto, @"\s+", " ");

            return texto.Trim();
        }

        /// <summary>
        /// Extrai, a partir do texto livre digitado pelo profissional,
        /// os discriminadores clínicos do Protocolo de Manchester,
        /// utilizando o léxico clínico controlado (termo canônico +
        /// sinônimos) descrito na seção 2.3 do projeto de pesquisa, com
        /// tolerância a variações de grafia.
        /// </summary>
        public List<DiscriminadorManchester> Extrair(
            string texto,
            List<DiscriminadorManchester> baseDados)
        {
            texto = Normalizar(texto);
            var palavrasDoTexto = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var encontrados = new Dictionary<string, (DiscriminadorManchester Discriminador, string TermoCasado)>();

            foreach (var discriminador in baseDados)
            {
                foreach (var termoOriginal in discriminador.ObterTermosBusca())
                {
                    var termo = Normalizar(termoOriginal);

                    if (string.IsNullOrWhiteSpace(termo))
                        continue;

                    bool negado = Negacoes.Any(n =>
                        Regex.IsMatch(
                            texto,
                            $@"\b{Regex.Escape(n)}\s+{Regex.Escape(termo)}\b"));

                    if (negado)
                        continue;

                    // 1) Correspondência exata (termo canônico ou sinônimo completo)
                    bool casouExato = Regex.IsMatch(texto, $@"\b{Regex.Escape(termo)}\b");

                    // 2) Correspondência aproximada (grafia incorreta), apenas
                    // para termos de uma única palavra, para reduzir ambiguidade.
                    bool casouAproximado = false;
                    if (!casouExato && !termo.Contains(' ') && termo.Length >= TamanhoMinimoParaFuzzy)
                    {
                        casouAproximado = palavrasDoTexto.Any(palavra =>
                            Math.Abs(palavra.Length - termo.Length) <= DistanciaMaximaTolerada &&
                            DistanciaLevenshtein(palavra, termo) <= DistanciaMaximaTolerada);
                    }

                    if (!casouExato && !casouAproximado)
                        continue;

                    var chaveDiscriminador = discriminador.PalavraChave.ToLowerInvariant();

                    if (!encontrados.ContainsKey(chaveDiscriminador))
                    {
                        encontrados.Add(chaveDiscriminador, (discriminador, termoOriginal));
                    }

                    break; // já encontrou este discriminador, passa para o próximo
                }
            }

            var resultado = FiltrarPorEspecificidade(encontrados);

            _logger.LogDebug(
                "PLN extraiu {Quantidade} discriminador(es) do texto informado.",
                resultado.Count);

            return resultado
                .OrderByDescending(d => d.Gravidade)
                .ToList();
        }

        /// <summary>
        /// Quando dois discriminadores casam no mesmo trecho do texto e o
        /// termo de um está contido dentro do termo do outro (ex.:
        /// "dispneia" dentro de "dispneia leve"), mantém apenas o
        /// discriminador mais específico (termo mais longo/completo),
        /// em vez de deixar a Regra 6 escolher automaticamente o mais
        /// grave entre os dois. Isso evita que um termo genérico e mais
        /// grave "engula" um termo mais específico e mais brando que o
        /// profissional realmente digitou.
        /// </summary>
        private List<DiscriminadorManchester> FiltrarPorEspecificidade(
            Dictionary<string, (DiscriminadorManchester Discriminador, string TermoCasado)> encontrados)
        {
            var itens = encontrados.Values
                .Select(v => (v.Discriminador, TermoNormalizado: Normalizar(v.TermoCasado)))
                .ToList();

            var chavesRemovidas = new HashSet<string>();

            foreach (var candidatoMenosEspecifico in itens)
            {
                foreach (var candidatoMaisEspecifico in itens)
                {
                    if (ReferenceEquals(candidatoMenosEspecifico.Discriminador, candidatoMaisEspecifico.Discriminador))
                        continue;

                    var termoCurto = candidatoMenosEspecifico.TermoNormalizado;
                    var termoLongo = candidatoMaisEspecifico.TermoNormalizado;

                    var estaContido = termoLongo.Length > termoCurto.Length &&
                        Regex.IsMatch(termoLongo, $@"\b{Regex.Escape(termoCurto)}\b");

                    if (estaContido)
                    {
                        var chave = candidatoMenosEspecifico.Discriminador.PalavraChave.ToLowerInvariant();
                        chavesRemovidas.Add(chave);
                    }
                }
            }

            if (chavesRemovidas.Any())
            {
                _logger.LogDebug(
                    "Discriminador(es) descartado(s) por haver correspondência mais específica: {Chaves}",
                    string.Join(", ", chavesRemovidas));
            }

            return itens
                .Where(i => !chavesRemovidas.Contains(i.Discriminador.PalavraChave.ToLowerInvariant()))
                .Select(i => i.Discriminador)
                .ToList();
        }

        /// <summary>
        /// Calcula a distância de edição (Levenshtein) entre duas
        /// palavras — utilizada para tolerar pequenas variações de
        /// grafia (erros de digitação) no texto livre, conforme
        /// descrito na seção 2.3 do projeto ("grafias incorretas de
        /// termos relevantes são mapeados para formas canônicas").
        /// </summary>
        private static int DistanciaLevenshtein(string a, string b)
        {
            var custos = new int[b.Length + 1];

            for (int j = 0; j <= b.Length; j++)
                custos[j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                custos[0] = i;
                int anterior = i - 1;

                for (int j = 1; j <= b.Length; j++)
                {
                    int atual = custos[j];

                    custos[j] = Math.Min(
                        Math.Min(custos[j] + 1, custos[j - 1] + 1),
                        anterior + (a[i - 1] == b[j - 1] ? 0 : 1));

                    anterior = atual;
                }
            }

            return custos[b.Length];
        }

        public List<DiscriminadorManchester> AvaliarSinaisVitais(
            int? fc,
            int? fr,
            int? pas,
            int? spo2,
            double? temperatura)
        {
            _logger.LogDebug(
                "Sinais vitais informados: FC={Fc} FR={Fr} PAS={Pas} SpO2={Spo2} Temp={Temp}",
                fc, fr, pas, spo2, temperatura);

            var lista = new List<DiscriminadorManchester>();

            // Vermelho
            if (spo2 < 90)
                lista.Add(Novo(
                    "SpO₂ < 90%",
                    CorTriagem.Vermelho,
                    5,
                    "Hipoxemia grave"));

            if (pas < 90)
                lista.Add(Novo(
                    "PAS < 90",
                    CorTriagem.Vermelho,
                    5,
                    "Hipotensão grave"));

            if (fr > 35)
                lista.Add(Novo(
                    "FR > 35",
                    CorTriagem.Vermelho,
                    5,
                    "Taquipneia extrema"));

            // Laranja
            if (temperatura >= 39)
                lista.Add(Novo(
                    "Temperatura ≥ 39°C",
                    CorTriagem.Laranja,
                    4,
                    "Febre alta"));

            if (fc > 130)
                lista.Add(Novo(
                    "FC > 130",
                    CorTriagem.Laranja,
                    4,
                    "Taquicardia importante"));

            if (fr >= 30 && fr <= 35)
                lista.Add(Novo(
                    "FR 30-35",
                    CorTriagem.Laranja,
                    4,
                    "Taquipneia importante"));

            // Amarelo
            if (fc > 100 && fc <= 130)
                lista.Add(Novo(
                    "FC 101-130",
                    CorTriagem.Amarelo,
                    3,
                    "Taquicardia moderada"));

            if (fr >= 20 && fr < 30)
                lista.Add(Novo(
                    "FR 20-29",
                    CorTriagem.Amarelo,
                    3,
                    "Frequência respiratória aumentada"));

            if (temperatura >= 37.5 && temperatura < 39)
                lista.Add(Novo(
                    "Febre moderada",
                    CorTriagem.Amarelo,
                    3,
                    "Temperatura elevada"));

            if (lista.Count > 0)
            {
                _logger.LogDebug(
                    "Discriminadores de sinais vitais encontrados: {Discriminadores}",
                    string.Join(", ", lista.Select(d => $"{d.Nome} ({d.Cor})")));
            }

            return lista
                .OrderByDescending(x => x.Gravidade)
                .ToList();
        }

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