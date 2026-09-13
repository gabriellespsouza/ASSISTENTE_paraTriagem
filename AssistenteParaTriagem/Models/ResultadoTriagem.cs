using System.Collections.Generic;

namespace AssistenteParaTriagem.Models
{
    public class ResultadoTriagem
    {
        public CorTriagem Cor { get; set; }

        public int TempoMaximo { get; set; }

        public string Justificativa { get; set; } =
            string.Empty;

        public List<DiscriminadorManchester> Discriminadores
        {
            get;
            set;
        } = new();

        public List<string> RegrasAplicadas
        {
            get;
            set;
        } = new();

        public string Aviso { get; set; } =
            "Esta classificação é uma sugestão de apoio à decisão e não substitui a avaliação profissional.";

        public static ResultadoTriagem Vermelho(
            string justificativa) =>
            Criar(
                CorTriagem.Vermelho,
                0,
                justificativa);

        public static ResultadoTriagem Laranja(
            string justificativa) =>
            Criar(
                CorTriagem.Laranja,
                10,
                justificativa);

        public static ResultadoTriagem Amarelo(
            string justificativa) =>
            Criar(
                CorTriagem.Amarelo,
                60,
                justificativa);

        public static ResultadoTriagem Verde(
            string justificativa) =>
            Criar(
                CorTriagem.Verde,
                120,
                justificativa);

        public static ResultadoTriagem Azul(
            string justificativa) =>
            Criar(
                CorTriagem.Azul,
                240,
                justificativa);

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