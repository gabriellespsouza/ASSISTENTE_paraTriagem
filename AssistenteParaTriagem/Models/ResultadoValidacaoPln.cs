namespace AssistenteParaTriagem.Models
{
    public class ResultadoValidacaoPln
    {
        public int TotalCenarios { get; set; }

        public int VerdadeirosPositivos { get; set; }

        public int FalsosPositivos { get; set; }

        public int FalsosNegativos { get; set; }

        public double Precisao { get; set; }

        public double Recall { get; set; }

        public double F1 { get; set; }
    }
}
