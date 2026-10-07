namespace AssistenteParaTriagem.Models
{
    public class ResultadoMetricas
    {
        public int TotalCasos { get; set; }

        public int AcertosSistema { get; set; }

        public double AcuraciaSistema { get; set; }

        public double Precisao { get; set; }

        public double Recall { get; set; }

        public double F1 { get; set; }

        public double Kappa { get; set; }

        public double UnderTriage { get; set; }

        public double OverTriage { get; set; }

        public int PlnTotalCenarios { get; set; }

        public int PlnVerdadeirosPositivos { get; set; }

        public int PlnFalsosPositivos { get; set; }

        public int PlnFalsosNegativos { get; set; }

        public double PlnPrecisao { get; set; }

        public double PlnRecall { get; set; }

        public double PlnF1 { get; set; }
    }
}