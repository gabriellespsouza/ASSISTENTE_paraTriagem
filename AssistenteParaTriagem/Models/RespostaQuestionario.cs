using System.ComponentModel.DataAnnotations;

namespace AssistenteParaTriagem.Models
{
    public class RespostaQuestionario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string NomeProfissional { get; set; } = string.Empty;

        [Range(1, 5)]
        public int FacilidadeUso { get; set; }

        [Range(1, 5)]
        public int ClarezaRecomendacao { get; set; }

        [Range(1, 5)]
        public int Utilidade { get; set; }

        [Range(1, 5)]
        public int Confianca { get; set; }

        [Range(1, 5)]
        public int RecomendariaUso { get; set; }

        [StringLength(2000)]
        public string PontosPositivos { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Dificuldades { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Sugestoes { get; set; } = string.Empty;

        public DateTime DataHora { get; set; } = DateTime.Now;
    }
}