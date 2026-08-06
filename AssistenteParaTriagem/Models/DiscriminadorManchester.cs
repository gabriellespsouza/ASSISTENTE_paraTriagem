using System.ComponentModel.DataAnnotations;

namespace AssistenteParaTriagem.Models
{
    /// <summary>
    /// Representa um discriminador clínico utilizado pelo
    /// Sistema Manchester.
    /// </summary>
    public class DiscriminadorManchester
    {
        /// <summary>
        /// Termo canônico (palavra ou expressão principal) utilizado
        /// pelo PLN para identificar o discriminador.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string PalavraChave { get; set; } = string.Empty;

        /// <summary>
        /// Léxico clínico controlado: sinônimos, abreviações, variações
        /// coloquiais e grafias alternativas do termo canônico,
        /// separados por "|". Ex.: "cefalalgia|dorzinha na cabeca|cabeca latejando".
        /// Todos são mapeados para o mesmo discriminador clínico.
        /// </summary>
        [StringLength(1000)]
        public string Sinonimos { get; set; } = string.Empty;

        /// <summary>
        /// Nome clínico do discriminador.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Cor de risco associada ao discriminador.
        /// </summary>
        [Required]
        public CorTriagem Cor { get; set; }

        /// <summary>
        /// Nível de gravidade utilizado para ordenar
        /// os discriminadores encontrados.
        /// </summary>
        [Range(1, 5)]
        public int Gravidade { get; set; }

        /// <summary>
        /// Justificativa clínica apresentada ao usuário.
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string Justificativa { get; set; } = string.Empty;

        /// <summary>
        /// Retorna todos os termos de busca (termo canônico + sinônimos)
        /// que compõem o léxico clínico controlado deste discriminador,
        /// conforme descrito na seção 2.3 do projeto de pesquisa.
        /// </summary>
        public IEnumerable<string> ObterTermosBusca()
        {
            if (!string.IsNullOrWhiteSpace(PalavraChave))
                yield return PalavraChave;

            if (string.IsNullOrWhiteSpace(Sinonimos))
                yield break;

            foreach (var sinonimo in Sinonimos.Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                var termo = sinonimo.Trim();
                if (!string.IsNullOrWhiteSpace(termo))
                    yield return termo;
            }
        }
    }
}