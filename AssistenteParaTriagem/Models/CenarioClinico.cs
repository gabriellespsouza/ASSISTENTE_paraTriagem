using System.ComponentModel.DataAnnotations;

namespace AssistenteParaTriagem.Models
{
    public class CenarioClinico
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string QueixaPrincipal { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Sintomas { get; set; } = string.Empty;

        [StringLength(200)]
        public string TempoEvolucao { get; set; } = string.Empty;

        public int? FrequenciaCardiaca { get; set; }

        public int? FrequenciaRespiratoria { get; set; }

        public int? PressaoSistolica { get; set; }

        public int? Saturacao { get; set; }

        public double? Temperatura { get; set; }

        public bool PacienteInconsciente { get; set; }

        [StringLength(2000)]
        public string DiscriminadoresEsperados { get; set; } = string.Empty;

        [Required]
        public CorTriagem CorPadraoOuro { get; set; }

        [StringLength(2000)]
        public string Observacoes { get; set; } = string.Empty;
    }
}