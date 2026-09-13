using System.ComponentModel.DataAnnotations;

namespace AssistenteParaTriagem.Models
{
    public class AvaliacaoCenario
    {
        public int Id { get; set; }

        [Required]
        public int CenarioClinicoId { get; set; }

        public CenarioClinico? CenarioClinico { get; set; }

        [Required]
        [StringLength(150)]
        public string NomeProfissional { get; set; } = string.Empty;

        [Required]
        public CorTriagem CorProfissional { get; set; }

        [Required]
        public CorTriagem CorSistema { get; set; }

        [Required]
        public CorTriagem CorPadraoOuro { get; set; }

        public bool SistemaAcertou { get; set; }

        public bool ProfissionalAcertou { get; set; }

        public bool Subtriagem { get; set; }

        public bool Supertriagem { get; set; }

        [StringLength(3000)]
        public string DiscriminadoresIdentificados { get; set; } = string.Empty;

        [StringLength(3000)]
        public string RegrasAplicadas { get; set; } = string.Empty;

        [StringLength(3000)]
        public string JustificativaSistema { get; set; } = string.Empty;

        public DateTime DataHora { get; set; } = DateTime.Now;
    }
}