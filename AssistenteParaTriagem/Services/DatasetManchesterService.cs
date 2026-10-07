using AssistenteParaTriagem.Models;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace AssistenteParaTriagem.Services
{
    public class DatasetManchesterService
    {
        public List<DiscriminadorManchester> Discriminadores { get; private set; }

        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DatasetManchesterService> _logger;

        public DatasetManchesterService(
            IWebHostEnvironment environment,
            ILogger<DatasetManchesterService> logger)
        {
            _environment = environment;
            _logger = logger;
            Discriminadores = CarregarDiscriminadores();
        }

        private List<DiscriminadorManchester> CarregarDiscriminadores()
        {
            try
            {
                var caminho = Path.Combine(
                    _environment.ContentRootPath,
                    "Data",
                    "manchester.csv");

                if (!File.Exists(caminho))
                {
                    throw new FileNotFoundException(
                        $"Arquivo não encontrado: {caminho}");
                }

                // Lê os bytes e decodifica de forma robusta: tenta UTF-8
                // estrito e, se o arquivo estiver em ANSI (Windows-1252),
                // usa Latin1. Evita acentos quebrados no léxico.
                var bytes = File.ReadAllBytes(caminho);
                string conteudo;

                try
                {
                    conteudo = new UTF8Encoding(false, true).GetString(bytes);
                }
                catch (DecoderFallbackException)
                {
                    conteudo = Encoding.Latin1.GetString(bytes);

                    _logger.LogWarning(
                        "manchester.csv não está em UTF-8; lido como Latin1. " +
                        "Salve o arquivo em UTF-8 para evitar problemas.");
                }

                conteudo = conteudo.TrimStart('\uFEFF');

                using var reader = new StringReader(conteudo);

                var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    // Não derruba a leitura inteira se uma coluna nova (Sinonimos)
                    // ainda não existir no CSV do usuário ou se sobrar alguma
                    // coluna extra — evita "lista vazia silenciosa".
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                using var csv = new CsvReader(reader, config);

                var registros = csv
                    .GetRecords<DiscriminadorManchester>()
                    .Where(d =>
                        !string.IsNullOrWhiteSpace(d.PalavraChave) &&
                        !string.IsNullOrWhiteSpace(d.Nome))
                    .ToList();

                if (registros.Count == 0)
                {
                    _logger.LogWarning(
                        "ATENÇÃO: manchester.csv foi encontrado em {Caminho}, mas 0 " +
                        "discriminadores válidos foram carregados. Verifique se o " +
                        "cabeçalho contém PalavraChave, Sinonimos, Nome, Cor, " +
                        "Gravidade, Justificativa e se o arquivo tem 'Copy to Output " +
                        "Directory' = 'Copy if newer'.",
                        caminho);
                }
                else
                {
                    _logger.LogInformation(
                        "Léxico clínico controlado carregado: {Quantidade} discriminadores de {Caminho}.",
                        registros.Count,
                        caminho);

                    var duplicados = registros
                        .GroupBy(d => d.PalavraChave.Trim().ToLowerInvariant())
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (duplicados.Any())
                    {
                        _logger.LogWarning(
                            "ATENÇÃO: manchester.csv tem PalavraChave duplicada para: {Termos}. " +
                            "Apenas a primeira ocorrência de cada uma será usada pelo PLN — " +
                            "as demais linhas serão ignoradas silenciosamente. Use a coluna " +
                            "Sinonimos para unificar variações do mesmo termo em vez de repetir " +
                            "a PalavraChave.",
                            string.Join(", ", duplicados));
                    }
                }

                return registros;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao carregar dataset Manchester.");

                return new List<DiscriminadorManchester>();
            }
        }
    }
}