using System.ComponentModel.DataAnnotations;

namespace AssistenteParaTriagem.Models
{
    /// <summary>
    /// Registro de auditoria de uma decisão realizada pelo assistente
    /// inteligente de triagem.
    ///
    /// Armazena as entradas utilizadas na decisão, os discriminadores
    /// identificados pelo PLN, as regras acionadas pelo motor de regras,
    /// a classificação final e a justificativa apresentada ao usuário.
    /// </summary>
    public class AuditLogs
    {
        [Key]
        public int Id { get; set; }

        // ==========================================
        // Identificação do usuário
        // ==========================================

        /// <summary>
        /// Identificador do usuário autenticado no ASP.NET Identity.
        /// </summary>
        [StringLength(450)]
        public string? UserId { get; set; }

        /// <summary>
        /// Nome/e-mail apresentado para identificação do profissional.
        /// </summary>
        [Required]
        [StringLength(150)]
        public string NomeProfissional { get; set; } = string.Empty;

        // ==========================================
        // Data e hora da operação
        // ==========================================

        [Required]
        public DateTime DataHora { get; set; } = DateTime.Now;

        // ==========================================
        // Entradas fornecidas pelo profissional
        // ==========================================

        [Required]
        [StringLength(500)]
        public string Queixa { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Sintomas { get; set; } = string.Empty;

        // ==========================================
        // Sinais vitais
        // ==========================================

        public int? FrequenciaCardiaca { get; set; }

        public int? FrequenciaRespiratoria { get; set; }

        public int? PressaoSistolica { get; set; }

        public int? Saturacao { get; set; }

        public double? Temperatura { get; set; }

        public bool PacienteInconsciente { get; set; }

        // ==========================================
        // Resultado do processamento de linguagem
        // ==========================================

        /// <summary>
        /// Discriminadores identificados pelo PLN.
        /// </summary>
        [StringLength(3000)]
        public string Discriminadores { get; set; } = string.Empty;

        // ==========================================
        // Motor de regras
        // ==========================================

        /// <summary>
        /// Regras acionadas durante a classificação.
        /// </summary>
        [StringLength(3000)]
        public string RegrasAplicadas { get; set; } = string.Empty;

        // ==========================================
        // Resultado final
        // ==========================================

        [Required]
        public CorTriagem CorRisco { get; set; }

        public int TempoMaximo { get; set; }

        /// <summary>
        /// Explicação apresentada ao profissional sobre
        /// o motivo da classificação.
        /// </summary>
        [Required]
        [StringLength(3000)]
        public string Justificativa { get; set; } = string.Empty;

        // ==========================================
        // Identificação da operação
        // ==========================================

        [Required]
        [StringLength(100)]
        public string TipoOperacao { get; set; } = "Triagem";

        /// <summary>
        /// Indica que o registro representa uma decisão
        /// concluída pelo assistente.
        /// </summary>
        public bool DecisaoConcluida { get; set; } = true;
    }
}
