using System.ComponentModel.DataAnnotations;

namespace AssistenteParaTriagem.Models
{
    /// <summary>
    /// Representa os sinais vitais informados durante a triagem.
    /// </summary>
    public class SinaisVitais
    {
        /// <summary>
        /// Frequência cardíaca (bpm).
        /// </summary>
        [Range(0, 300)]
        public int? FrequenciaCardiaca { get; set; }

        /// <summary>
        /// Frequência respiratória (irpm).
        /// </summary>
        [Range(0, 100)]
        public int? FrequenciaRespiratoria { get; set; }

        /// <summary>
        /// Pressão arterial sistólica (mmHg).
        /// </summary>
        [Range(0, 300)]
        public int? PressaoSistolica { get; set; }

        /// <summary>
        /// Saturação periférica de oxigênio (%).
        /// </summary>
        [Range(0, 100)]
        public int? Saturacao { get; set; }

        /// <summary>
        /// Temperatura corporal (°C).
        /// </summary>
        [Range(25, 45)]
        public double? Temperatura { get; set; }

        /// <summary>
        /// Indica se o paciente está inconsciente.
        /// </summary>
        public bool PacienteInconsciente { get; set; }

        /// <summary>
        /// Verifica se pelo menos um sinal vital foi informado.
        /// </summary>
        public bool PossuiSinaisVitais =>
            FrequenciaCardiaca.HasValue ||
            FrequenciaRespiratoria.HasValue ||
            PressaoSistolica.HasValue ||
            Saturacao.HasValue ||
            Temperatura.HasValue;

        /// <summary>
        /// Indica, de forma amigável, se algum sinal vital está fora
        /// da faixa considerada normal — usado para compor a
        /// justificativa estruturada exigida na seção 2.3/3 do projeto.
        /// </summary>
        public bool PossuiAlteracao(List<string> descricoesAlteradas)
        {
            var alterado = false;

            if (Saturacao is < 95)
            {
                descricoesAlteradas.Add($"SpO₂ {Saturacao}%");
                alterado = true;
            }

            if (PressaoSistolica is < 90 or > 140)
            {
                descricoesAlteradas.Add($"PAS {PressaoSistolica} mmHg");
                alterado = true;
            }

            if (FrequenciaCardiaca is > 100 or < 50)
            {
                descricoesAlteradas.Add($"FC {FrequenciaCardiaca} bpm");
                alterado = true;
            }

            if (FrequenciaRespiratoria is > 20 or < 10)
            {
                descricoesAlteradas.Add($"FR {FrequenciaRespiratoria} irpm");
                alterado = true;
            }

            if (Temperatura is >= 37.5 or < 35.5)
            {
                descricoesAlteradas.Add($"Temperatura {Temperatura}°C");
                alterado = true;
            }

            return alterado;
        }
    }
}