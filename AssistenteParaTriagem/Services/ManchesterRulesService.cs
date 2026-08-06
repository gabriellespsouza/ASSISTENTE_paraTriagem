using AssistenteParaTriagem.Models;

namespace AssistenteParaTriagem.Services
{
    public class ManchesterRulesService
    {
        public ResultadoTriagem Avaliar(
            List<DiscriminadorManchester> discriminadores,
            SinaisVitais? vitais)
        {
            discriminadores ??= new();

            var descricoesAlteradas = new List<string>();
            vitais?.PossuiAlteracao(descricoesAlteradas);

            // ==========================================
            // REGRA 1 - Paciente inconsciente
            // ==========================================
            if (vitais?.PacienteInconsciente == true)
            {
                var resultado = ResultadoTriagem.Vermelho(
                    ConstruirJustificativa(
                        CorTriagem.Vermelho,
                        "paciente inconsciente",
                        "Rebaixamento do nível de consciência",
                        descricoesAlteradas));

                resultado.RegrasAplicadas.Add("Paciente inconsciente");
                resultado.Discriminadores = discriminadores;

                return resultado;
            }

            // ==========================================
            // REGRA 2 - Hipoxemia grave
            // ==========================================
            if (vitais?.Saturacao < 90)
            {
                var resultado = ResultadoTriagem.Vermelho(
                    ConstruirJustificativa(
                        CorTriagem.Vermelho,
                        $"saturação de O₂ {vitais.Saturacao}%",
                        "SpO₂ < 90%",
                        descricoesAlteradas));

                resultado.RegrasAplicadas.Add("SpO₂ < 90%");
                resultado.Discriminadores = discriminadores;

                return resultado;
            }

            // ==========================================
            // REGRA 3 - Hipotensão grave
            // ==========================================
            if (vitais?.PressaoSistolica < 90)
            {
                var resultado = ResultadoTriagem.Vermelho(
                    ConstruirJustificativa(
                        CorTriagem.Vermelho,
                        $"pressão sistólica {vitais.PressaoSistolica} mmHg",
                        "PAS < 90 mmHg",
                        descricoesAlteradas));

                resultado.RegrasAplicadas.Add("PAS < 90 mmHg");
                resultado.Discriminadores = discriminadores;

                return resultado;
            }

            // ==========================================
            // REGRA 4 - Taquipneia extrema
            // ==========================================
            if (vitais?.FrequenciaRespiratoria > 35)
            {
                var resultado = ResultadoTriagem.Vermelho(
                    ConstruirJustificativa(
                        CorTriagem.Vermelho,
                        $"frequência respiratória {vitais.FrequenciaRespiratoria} irpm",
                        "FR > 35 irpm",
                        descricoesAlteradas));

                resultado.RegrasAplicadas.Add("FR > 35 irpm");
                resultado.Discriminadores = discriminadores;

                return resultado;
            }

            // ==========================================
            // REGRA 5 - Suspeita de Sepse
            // ==========================================
            if (vitais != null &&
                vitais.Temperatura >= 39 &&
                vitais.FrequenciaCardiaca >= 130 &&
                vitais.FrequenciaRespiratoria >= 22 &&
                vitais.PressaoSistolica >= 90 &&
                vitais.Saturacao >= 90)
            {
                var resultado = ResultadoTriagem.Laranja(
                    ConstruirJustificativa(
                        CorTriagem.Laranja,
                        "febre alta associada a taquicardia e taquipneia",
                        "Critérios clínicos combinados para sepse",
                        descricoesAlteradas));

                resultado.RegrasAplicadas.Add("Critérios clínicos para sepse");
                resultado.Discriminadores = discriminadores;

                return resultado;
            }

            // ==========================================
            // REGRA 6 - Discriminadores do léxico clínico (PLN)
            // ==========================================
            if (discriminadores.Any())
            {
                var maisGrave = discriminadores
                    .OrderByDescending(x => x.Gravidade)
                    .First();

                var resultado = CriarResultado(maisGrave, descricoesAlteradas);
                resultado.Discriminadores = discriminadores;

                resultado.RegrasAplicadas.Add($"Discriminador: {maisGrave.Nome}");
                return resultado;
            }

            // ==========================================
            // REGRA 7 - Sem gravidade
            // ==========================================
            var semGravidade = ResultadoTriagem.Verde(
                "Classificação VERDE atribuída porque nenhum sintoma relatado ou " +
                "sinal vital informado correspondeu a um discriminador de gravidade " +
                "do Protocolo de Manchester.");

            semGravidade.Discriminadores = discriminadores;
            semGravidade.RegrasAplicadas.Add("Nenhum discriminador encontrado");

            return semGravidade;
        }

        private ResultadoTriagem CriarResultado(
            DiscriminadorManchester discriminador,
            List<string> descricoesAlteradas)
        {
            var justificativa = ConstruirJustificativa(
                discriminador.Cor,
                discriminador.Nome.ToLowerInvariant(),
                discriminador.Nome,
                descricoesAlteradas,
                justificativaClinicaAdicional: discriminador.Justificativa);

            return discriminador.Cor switch
            {
                CorTriagem.Vermelho => ResultadoTriagem.Vermelho(justificativa),
                CorTriagem.Laranja => ResultadoTriagem.Laranja(justificativa),
                CorTriagem.Amarelo => ResultadoTriagem.Amarelo(justificativa),
                CorTriagem.Verde => ResultadoTriagem.Verde(justificativa),
                CorTriagem.Azul => ResultadoTriagem.Azul(justificativa),
                _ => ResultadoTriagem.Verde("Sem sinais de gravidade."),
            };
        }

        /// <summary>
        /// Monta a justificativa no formato estruturado descrito na
        /// seção 3 do projeto de pesquisa, por exemplo:
        /// "Classificação AMARELA atribuída porque o sintoma 'dor intensa'
        /// corresponde ao discriminador X do protocolo, associado a sinais
        /// vitais alterados (PAS 88 mmHg, FC 118 bpm)."
        /// </summary>
        private static string ConstruirJustificativa(
            CorTriagem cor,
            string sintomaOuAchado,
            string nomeDiscriminador,
            List<string> descricoesAlteradas,
            string? justificativaClinicaAdicional = null)
        {
            var corTexto = cor.ToString().ToUpperInvariant();

            var trechoVitais = descricoesAlteradas.Any()
                ? $", associado a sinais vitais alterados ({string.Join(", ", descricoesAlteradas)})"
                : ", sem alterações relevantes nos sinais vitais informados";

            var complemento = string.IsNullOrWhiteSpace(justificativaClinicaAdicional)
                ? string.Empty
                : $" ({justificativaClinicaAdicional})";

            return $"Classificação {corTexto} atribuída porque o achado " +
                   $"'{sintomaOuAchado}' corresponde ao discriminador " +
                   $"'{nomeDiscriminador}' do Protocolo de Manchester{complemento}" +
                   $"{trechoVitais}.";
        }
    }
}