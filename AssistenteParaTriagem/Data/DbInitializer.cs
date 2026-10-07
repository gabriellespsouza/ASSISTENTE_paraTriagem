using AssistenteParaTriagem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssistenteParaTriagem.Data
{
    public static class DbInitializer
    {
        public static async Task InicializarAsync(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager = null)
        {
            await context.Database.MigrateAsync();

            if (!context.CenariosClinicos.Any())
            {
                var cenarios = new List<CenarioClinico>
{
    // =========================================================
    // CASO 1 - ADULTO INDISPOSTO / GRAVE
    // Baseado em estudos sobre o fluxograma "Unwell adult"
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Adulto com alteração importante do estado geral",
        QueixaPrincipal = "Mal-estar intenso",
        Sintomas = "Paciente apresenta fraqueza intensa, confusão e piora progressiva do estado geral.",
        TempoEvolucao = "Há algumas horas",
        FrequenciaCardiaca = 128,
        FrequenciaRespiratoria = 30,
        PressaoSistolica = 88,
        Saturacao = 91,
        Temperatura = 39.2,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Mal-estar geral, Alteração importante da consciência",
        CorPadraoOuro = CorTriagem.Vermelho
    },

    // =========================================================
    // CASO 2 - DISPNEIA GRAVE
    // Baseado em estudos do fluxograma Shortness of Breath
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Dispneia com baixa saturação",
        QueixaPrincipal = "Falta de ar",
        Sintomas = "Paciente apresenta dificuldade importante para respirar e cansaço aos mínimos esforços.",
        TempoEvolucao = "Há 2 horas",
        FrequenciaCardiaca = 125,
        FrequenciaRespiratoria = 34,
        PressaoSistolica = 105,
        Saturacao = 87,
        Temperatura = 37.1,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Dispneia",
        CorPadraoOuro = CorTriagem.Vermelho
    },

    // =========================================================
    // CASO 3 - DOR TORÁCICA IMPORTANTE
    // Baseado em estudos do fluxograma Chest Pain
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Dor torácica com sintomas associados",
        QueixaPrincipal = "Dor no peito",
        Sintomas = "Dor torácica intensa acompanhada de sudorese e sensação de fraqueza.",
        TempoEvolucao = "Há 40 minutos",
        FrequenciaCardiaca = 112,
        FrequenciaRespiratoria = 24,
        PressaoSistolica = 105,
        Saturacao = 94,
        Temperatura = 36.7,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Dor torácica intensa, Sudorese",
        CorPadraoOuro = CorTriagem.Laranja
    },

    // =========================================================
    // CASO 4 - DOR ABDOMINAL AGUDA
    // Baseado no estudo de Zaboli et al. sobre dor abdominal
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Dor abdominal aguda intensa",
        QueixaPrincipal = "Dor abdominal intensa",
        Sintomas = "Dor abdominal forte, contínua e de início recente.",
        TempoEvolucao = "Há 3 horas",
        FrequenciaCardiaca = 110,
        FrequenciaRespiratoria = 23,
        PressaoSistolica = 105,
        Saturacao = 96,
        Temperatura = 37.8,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Dor abdominal intensa",
        CorPadraoOuro = CorTriagem.Laranja
    },

    // =========================================================
    // CASO 5 - FEBRE COM SUSPEITA DE INFECÇÃO GRAVE
    // Baseado em estudos sobre febre e MTS
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Febre elevada com sinais sistêmicos",
        QueixaPrincipal = "Febre alta",
        Sintomas = "Febre elevada associada a prostração, taquicardia e respiração acelerada.",
        TempoEvolucao = "Há 2 dias",
        FrequenciaCardiaca = 135,
        FrequenciaRespiratoria = 27,
        PressaoSistolica = 98,
        Saturacao = 93,
        Temperatura = 40.0,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Febre alta, Taquicardia, Taquipneia",
        CorPadraoOuro = CorTriagem.Laranja
    },

    // =========================================================
    // CASO 6 - SEPSE / ALTERAÇÃO DO ESTADO MENTAL
    // Baseado nos estudos de Gräff et al. e Dewitte et al.
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Suspeita de sepse com alteração mental",
        QueixaPrincipal = "Febre e confusão",
        Sintomas = "Paciente com febre, confusão mental, fraqueza intensa e piora do estado geral.",
        TempoEvolucao = "Há 8 horas",
        FrequenciaCardiaca = 130,
        FrequenciaRespiratoria = 28,
        PressaoSistolica = 92,
        Saturacao = 93,
        Temperatura = 39.5,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Febre, Alteração importante da consciência",
        CorPadraoOuro = CorTriagem.Laranja
    },

    // =========================================================
    // CASO 7 - HEMORRAGIA DIGESTIVA
    // Baseado no estudo de Nguyen-Tat et al.
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Sangramento gastrointestinal",
        QueixaPrincipal = "Vômito com sangue",
        Sintomas = "Paciente apresenta episódio de vômito com sangue acompanhado de fraqueza e tontura.",
        TempoEvolucao = "Há 1 hora",
        FrequenciaCardiaca = 125,
        FrequenciaRespiratoria = 25,
        PressaoSistolica = 90,
        Saturacao = 94,
        Temperatura = 36.4,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Sangramento importante, Tontura",
        CorPadraoOuro = CorTriagem.Laranja
    },

    // =========================================================
    // CASO 8 - SÍNCOPE
    // Baseado no estudo sobre perda transitória de consciência
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Perda transitória de consciência",
        QueixaPrincipal = "Desmaio",
        Sintomas = "Paciente apresentou perda transitória de consciência, recuperando-se espontaneamente.",
        TempoEvolucao = "Há 30 minutos",
        FrequenciaCardiaca = 105,
        FrequenciaRespiratoria = 20,
        PressaoSistolica = 100,
        Saturacao = 97,
        Temperatura = 36.6,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Síncope",
        CorPadraoOuro = CorTriagem.Amarelo
    },

    // =========================================================
    // CASO 9 - DOR ABDOMINAL MODERADA
    // Baseado em estudo sobre dor abdominal no MTS
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Dor abdominal moderada",
        QueixaPrincipal = "Dor abdominal",
        Sintomas = "Dor abdominal moderada, sem sinais clínicos de instabilidade.",
        TempoEvolucao = "Há 5 horas",
        FrequenciaCardiaca = 92,
        FrequenciaRespiratoria = 18,
        PressaoSistolica = 122,
        Saturacao = 98,
        Temperatura = 37.0,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Dor abdominal",
        CorPadraoOuro = CorTriagem.Amarelo
    },

    // =========================================================
    // CASO 10 - DOR TORÁCICA MODERADA
    // Baseado no fluxograma Chest Pain
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Dor torácica moderada",
        QueixaPrincipal = "Dor torácica",
        Sintomas = "Dor no peito de intensidade moderada, sem alteração importante dos sinais vitais.",
        TempoEvolucao = "Há 2 horas",
        FrequenciaCardiaca = 90,
        FrequenciaRespiratoria = 18,
        PressaoSistolica = 125,
        Saturacao = 98,
        Temperatura = 36.7,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Dor torácica moderada",
        CorPadraoOuro = CorTriagem.Amarelo
    },

    // =========================================================
    // CASO 11 - CEFALEIA COM SINAIS DE ALERTA
    // Baseado no estudo sobre subtriagem em cefaleia
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Cefaleia com sinal de alerta",
        QueixaPrincipal = "Dor de cabeça intensa",
        Sintomas = "Cefaleia intensa de início recente acompanhada de vômitos e alteração visual.",
        TempoEvolucao = "Há 2 horas",
        FrequenciaCardiaca = 96,
        FrequenciaRespiratoria = 19,
        PressaoSistolica = 150,
        Saturacao = 98,
        Temperatura = 36.8,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Cefaleia intensa, Vômitos",
        CorPadraoOuro = CorTriagem.Laranja
    },

    // =========================================================
    // CASO 12 - CEFALEIA LEVE
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Cefaleia sem sinais de alarme",
        QueixaPrincipal = "Dor de cabeça",
        Sintomas = "Dor de cabeça leve, sem alteração neurológica ou sinais de gravidade.",
        TempoEvolucao = "Há 1 dia",
        FrequenciaCardiaca = 78,
        FrequenciaRespiratoria = 16,
        PressaoSistolica = 118,
        Saturacao = 99,
        Temperatura = 36.6,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Dor de cabeça",
        CorPadraoOuro = CorTriagem.Verde
    },

    // =========================================================
    // CASO 13 - VÔMITOS
    // Baseado em estudos de queixas no pronto atendimento
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Vômitos sem instabilidade",
        QueixaPrincipal = "Vômitos",
        Sintomas = "Episódios de vômitos nas últimas horas, sem sinais de choque ou alteração da consciência.",
        TempoEvolucao = "Há 8 horas",
        FrequenciaCardiaca = 84,
        FrequenciaRespiratoria = 17,
        PressaoSistolica = 120,
        Saturacao = 98,
        Temperatura = 36.8,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Vômitos",
        CorPadraoOuro = CorTriagem.Verde
    },

    // =========================================================
    // CASO 14 - DIARREIA
    // Baseado em estudo de queixas e desfechos no MTS
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Diarreia sem sinais de instabilidade",
        QueixaPrincipal = "Diarreia",
        Sintomas = "Episódios de diarreia há dois dias, sem sangue e sem sinais de desidratação grave.",
        TempoEvolucao = "Há 2 dias",
        FrequenciaCardiaca = 88,
        FrequenciaRespiratoria = 17,
        PressaoSistolica = 118,
        Saturacao = 98,
        Temperatura = 37.2,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Diarreia",
        CorPadraoOuro = CorTriagem.Verde
    },

    // =========================================================
    // CASO 15 - PROBLEMA EM MEMBRO
    // Baseado em estudos em que "limb problems" é um dos
    // fluxogramas mais utilizados
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Dor em membro após esforço",
        QueixaPrincipal = "Dor no braço",
        Sintomas = "Dor localizada no braço após esforço, sem deformidade, sem perda de consciência e sem sinais sistêmicos.",
        TempoEvolucao = "Há 1 dia",
        FrequenciaCardiaca = 80,
        FrequenciaRespiratoria = 16,
        PressaoSistolica = 120,
        Saturacao = 99,
        Temperatura = 36.5,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Dor em membro",
        CorPadraoOuro = CorTriagem.Verde
    },

    // =========================================================
    // CASO 16 - QUEDA
    // Baseado em estudos que incluem o fluxograma Falls
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Queda sem instabilidade",
        QueixaPrincipal = "Queda",
        Sintomas = "Paciente sofreu queda da própria altura, apresenta dor leve no membro inferior e consegue caminhar.",
        TempoEvolucao = "Há 3 horas",
        FrequenciaCardiaca = 82,
        FrequenciaRespiratoria = 16,
        PressaoSistolica = 122,
        Saturacao = 98,
        Temperatura = 36.5,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Queda, Dor leve",
        CorPadraoOuro = CorTriagem.Verde
    },

    // =========================================================
    // CASO 17 - TRAUMA CRANIANO
    // Baseado em caso publicado sobre trauma craniano e MTS
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Trauma craniano com vômitos",
        QueixaPrincipal = "Trauma na cabeça",
        Sintomas = "Paciente sofreu trauma craniano e posteriormente apresentou vômitos e piora clínica.",
        TempoEvolucao = "Há 2 horas",
        FrequenciaCardiaca = 105,
        FrequenciaRespiratoria = 22,
        PressaoSistolica = 135,
        Saturacao = 96,
        Temperatura = 36.7,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Trauma craniano, Vômitos",
        CorPadraoOuro = CorTriagem.Laranja
    },

    // =========================================================
    // CASO 18 - TRAUMA GRAVE
    // Baseado nos fluxogramas de major trauma do MTS
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Trauma com comprometimento clínico",
        QueixaPrincipal = "Trauma grave",
        Sintomas = "Paciente vítima de trauma importante apresenta dor intensa, taquicardia e dificuldade respiratória.",
        TempoEvolucao = "Há poucos minutos",
        FrequenciaCardiaca = 128,
        FrequenciaRespiratoria = 32,
        PressaoSistolica = 88,
        Saturacao = 90,
        Temperatura = 36.2,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Trauma grave, Dor intensa, Taquicardia, Dispneia",
        CorPadraoOuro = CorTriagem.Vermelho
    },

    // =========================================================
    // CASO 19 - DOR LOMBAR
    // Baseado em estudos que utilizam o fluxograma Back Pain
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Dor lombar sem sinais de instabilidade",
        QueixaPrincipal = "Dor lombar",
        Sintomas = "Dor lombar moderada após esforço físico, sem déficit neurológico ou alteração dos sinais vitais.",
        TempoEvolucao = "Há 1 dia",
        FrequenciaCardiaca = 82,
        FrequenciaRespiratoria = 16,
        PressaoSistolica = 120,
        Saturacao = 99,
        Temperatura = 36.5,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Dor lombar",
        CorPadraoOuro = CorTriagem.Amarelo
    },

    // =========================================================
    // CASO 20 - SINTOMAS URINÁRIOS
    // Baseado em estudos que utilizam o fluxograma Urinary Problems
    // =========================================================
    new CenarioClinico
    {
        Titulo = "Sintomas urinários sem instabilidade",
        QueixaPrincipal = "Dor ao urinar",
        Sintomas = "Paciente apresenta ardência ao urinar e aumento da frequência urinária, sem sinais de instabilidade.",
        TempoEvolucao = "Há 2 dias",
        FrequenciaCardiaca = 86,
        FrequenciaRespiratoria = 16,
        PressaoSistolica = 120,
        Saturacao = 99,
        Temperatura = 37.4,
        PacienteInconsciente = false,
        DiscriminadoresEsperados = "Sintomas urinários",
        CorPadraoOuro = CorTriagem.Verde
    }
};
                await context.CenariosClinicos.AddRangeAsync(cenarios);
                await context.SaveChangesAsync();
            }
            // Sempre sincroniza o gabarito dos discriminadores esperados
            // com os nomes do manchester.csv (coluna Nome). Corrige bancos
            // já criados (triagem.db) sem precisar apagá-los.
            {
                var gabarito = new Dictionary<string, string>
                {
                    ["Adulto com alteração importante do estado geral"] = "Mal-estar geral, Alteração importante da consciência",
                    ["Dispneia com baixa saturação"] = "Dispneia",
                    ["Dor torácica com sintomas associados"] = "Dor torácica intensa, Sudorese",
                    ["Dor abdominal aguda intensa"] = "Dor abdominal intensa",
                    ["Febre elevada com sinais sistêmicos"] = "Febre alta, Taquicardia, Taquipneia",
                    ["Suspeita de sepse com alteração mental"] = "Febre, Alteração importante da consciência",
                    ["Sangramento gastrointestinal"] = "Sangramento importante, Tontura",
                    ["Perda transitória de consciência"] = "Síncope",
                    ["Dor abdominal moderada"] = "Dor abdominal",
                    ["Dor torácica moderada"] = "Dor torácica moderada",
                    ["Cefaleia com sinal de alerta"] = "Cefaleia intensa, Vômitos",
                    ["Cefaleia sem sinais de alarme"] = "Dor de cabeça",
                    ["Vômitos sem instabilidade"] = "Vômitos",
                    ["Diarreia sem sinais de instabilidade"] = "Diarreia",
                    ["Dor em membro após esforço"] = "Dor em membro",
                    ["Queda sem instabilidade"] = "Queda, Dor leve",
                    ["Trauma craniano com vômitos"] = "Trauma craniano, Vômitos",
                    ["Trauma com comprometimento clínico"] = "Trauma grave, Dor intensa, Taquicardia, Dispneia",
                    ["Dor lombar sem sinais de instabilidade"] = "Dor lombar",
                    ["Sintomas urinários sem instabilidade"] = "Sintomas urinários"
                };

                var existentes = await context.CenariosClinicos.ToListAsync();
                var alterou = false;

                foreach (var c in existentes)
                {
                    if (gabarito.TryGetValue(c.Titulo, out var esperado) &&
                        c.DiscriminadoresEsperados != esperado)
                    {
                        c.DiscriminadoresEsperados = esperado;
                        alterou = true;
                    }
                }

                if (alterou)
                {
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}