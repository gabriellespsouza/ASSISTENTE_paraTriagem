using System.ComponentModel.DataAnnotations;

namespace AssistenteParaTriagem.Models
{
    /// <summary>
    /// Registro de auditoria de uma triagem realizada, contemplando
    /// entradas, discriminadores, regras ativadas e classificação final,
    /// conforme exigido na seção 2.3 ("Toda interação... é registrada em
    /// logs de auditoria") e na seção 3 ("Material e Métodos") do
    /// projeto de pesquisa.
    /// </summary>
    public class AvaliacaoTriagem
    {
        [Key]
        public int Id { get; set; }

        // ==========================
        // Dados do profissional
        // ==========================
        [Required]
        [StringLength(150)]
        public string NomeProfissional { get; set; } = string.Empty;

        // ==========================
        // Dados da triagem (entradas)
        // ==========================
        [Required]
        [StringLength(500)]
        public string Queixa { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Sintomas { get; set; } = string.Empty;

        // ==========================
        // Sinais Vitais
        // ==========================
        public int? FrequenciaCardiaca { get; set; }

        public int? FrequenciaRespiratoria { get; set; }

        public int? PressaoSistolica { get; set; }

        public int? Saturacao { get; set; }

        public double? Temperatura { get; set; }

        public bool PacienteInconsciente { get; set; }

        // ==========================
        // Resultado do PLN
        // ==========================
        [StringLength(3000)]
        public string Discriminadores { get; set; } = string.Empty;

        // ==========================
        // Regras ativadas no motor de regras
        // (obrigatório para a trilha de auditoria — antes não era salvo)
        // ==========================
        [StringLength(2000)]
        public string RegrasAplicadas { get; set; } = string.Empty;

        // ==========================
        // Resultado da classificação
        // ==========================
        [Required]
        public CorTriagem CorRisco { get; set; }

        public int TempoMaximo { get; set; }

        [Required]
        [StringLength(2000)]
        public string Justificativa { get; set; } = string.Empty;

        // ==========================
        // Auditoria
        // ==========================
        public DateTime DataHora { get; set; } = DateTime.Now;
    }
}