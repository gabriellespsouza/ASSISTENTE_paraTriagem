using System.Collections.Generic;

namespace AssistenteParaTriagem.Models
{
    /// <summary>
    /// Resultado da classificação de risco.
    /// </summary>
    public class ResultadoTriagem
    {
        /// <summary>
        /// Cor de classificação do Sistema Manchester.
        /// </summary>
        public CorTriagem Cor { get; set; }

        /// <summary>
        /// Tempo máximo recomendado para atendimento (em minutos).
        /// </summary>
        public int TempoMaximo { get; set; }

        /// <summary>
        /// Justificativa clínica da classificação.
        /// </summary>
        public string Justificativa { get; set; } = string.Empty;

        /// <summary>
        /// Lista dos discriminadores encontrados pelo PLN.
        /// </summary>
        public List<DiscriminadorManchester> Discriminadores { get; set; } = new();

        /// <summary>
        /// Regras clínicas utilizadas para definir a classificação.
        /// </summary>
        public List<string> RegrasAplicadas { get; set; } = new();

        public static ResultadoTriagem Vermelho(string justificativa) =>
            Criar(CorTriagem.Vermelho, 0, justificativa);

        public static ResultadoTriagem Laranja(string justificativa) =>
            Criar(CorTriagem.Laranja, 10, justificativa);

        public static ResultadoTriagem Amarelo(string justificativa) =>
            Criar(CorTriagem.Amarelo, 60, justificativa);

        public static ResultadoTriagem Verde(string justificativa) =>
            Criar(CorTriagem.Verde, 120, justificativa);

        public static ResultadoTriagem Azul(string justificativa) =>
            Criar(CorTriagem.Azul, 240, justificativa);

        private static ResultadoTriagem Criar(
            CorTriagem cor,
            int tempo,
            string justificativa)
        {
            return new ResultadoTriagem
            {
                Cor = cor,
                TempoMaximo = tempo,
                Justificativa = justificativa
            };
        }
    }
}
