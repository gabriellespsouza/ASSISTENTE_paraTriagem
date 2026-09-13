using AssistenteParaTriagem.Models;
using Microsoft.EntityFrameworkCore;

namespace AssistenteParaTriagem.Data
{
    public static class DbInitializer
    {
        public static async Task InicializarAsync(
            ApplicationDbContext context)
        {
            // =====================================================
            // Aplica migrations
            // =====================================================

            await context.Database.MigrateAsync();

            // =====================================================
            // CENÁRIOS CLÍNICOS
            // =====================================================

            var cenarios = new List<CenarioClinico>
            {
                // =================================================
                // VERMELHO
                // =================================================

                new CenarioClinico
                {
                    Titulo =
                        "Paciente inconsciente",

                    QueixaPrincipal =
                        "Paciente desacordado",

                    Sintomas =
                        "Paciente sem resposta aos estímulos.",

                    TempoEvolucao =
                        "Início súbito",

                    FrequenciaCardiaca = 90,
                    FrequenciaRespiratoria = 10,
                    PressaoSistolica = 90,
                    Saturacao = 90,
                    Temperatura = 36.5,

                    PacienteInconsciente = true,

                    CorPadraoOuro =
                        CorTriagem.Vermelho,

                    DiscriminadoresEsperados =
                        "Paciente inconsciente"
                },

                new CenarioClinico
                {
                    Titulo =
                        "Convulsão ativa",

                    QueixaPrincipal =
                        "Convulsão",

                    Sintomas =
                        "Paciente apresenta crise convulsiva ativa.",

                    TempoEvolucao =
                        "Início recente",

                    FrequenciaCardiaca = 120,
                    FrequenciaRespiratoria = 25,
                    PressaoSistolica = 130,
                    Saturacao = 94,
                    Temperatura = 37.0,

                    PacienteInconsciente = false,

                    CorPadraoOuro =
                        CorTriagem.Vermelho,

                    DiscriminadoresEsperados =
                        "Convulsão ativa"
                },

                new CenarioClinico
                {
                    Titulo =
                        "Hemorragia grave",

                    QueixaPrincipal =
                        "Sangramento intenso",

                    Sintomas =
                        "Paciente apresenta hemorragia grave com sangramento abundante.",

                    TempoEvolucao =
                        "Início recente",

                    FrequenciaCardiaca = 125,
                    FrequenciaRespiratoria = 28,
                    PressaoSistolica = 85,
                    Saturacao = 92,
                    Temperatura = 36.2,

                    PacienteInconsciente = false,

                    CorPadraoOuro =
                        CorTriagem.Vermelho,

                    DiscriminadoresEsperados =
                        "Hemorragia grave"
                },

                // =================================================
                // LARANJA
                // =================================================

                new CenarioClinico
                {
                    Titulo =
                        "Dor torácica intensa",

                    QueixaPrincipal =
                        "Dor intensa no peito",

                    Sintomas =
                        "Dor torácica, pressão no peito e desconforto.",

                    TempoEvolucao =
                        "Há aproximadamente 30 minutos",

                    FrequenciaCardiaca = 110,
                    FrequenciaRespiratoria = 22,
                    PressaoSistolica = 135,
                    Saturacao = 95,
                    Temperatura = 36.8,

                    CorPadraoOuro =
                        CorTriagem.Laranja,

                    DiscriminadoresEsperados =
                        "Dor torácica intensa"
                },

                new CenarioClinico
                {
                    Titulo =
                        "Dispneia importante",

                    QueixaPrincipal =
                        "Falta de ar",

                    Sintomas =
                        "Dificuldade para respirar e cansaço ao respirar.",

                    TempoEvolucao =
                        "Há 2 horas",

                    FrequenciaCardiaca = 125,
                    FrequenciaRespiratoria = 32,
                    PressaoSistolica = 120,
                    Saturacao = 91,
                    Temperatura = 37.2,

                    CorPadraoOuro =
                        CorTriagem.Laranja,

                    DiscriminadoresEsperados =
                        "Dispneia"
                },

                new CenarioClinico
                {
                    Titulo =
                        "Febre alta",

                    QueixaPrincipal =
                        "Febre alta",

                    Sintomas =
                        "Febre, mal-estar e taquicardia.",

                    TempoEvolucao =
                        "Desde ontem",

                    FrequenciaCardiaca = 135,
                    FrequenciaRespiratoria = 24,
                    PressaoSistolica = 105,
                    Saturacao = 94,
                    Temperatura = 39.5,

                    CorPadraoOuro =
                        CorTriagem.Laranja,

                    DiscriminadoresEsperados =
                        "Febre alta"
                },

                new CenarioClinico
                {
                    Titulo =
                        "Dor abdominal intensa",

                    QueixaPrincipal =
                        "Dor abdominal intensa",

                    Sintomas =
                        "Dor abdominal intensa e persistente com desconforto.",

                    TempoEvolucao =
                        "Há 2 horas",

                    FrequenciaCardiaca = 115,
                    FrequenciaRespiratoria = 22,
                    PressaoSistolica = 110,
                    Saturacao = 96,
                    Temperatura = 37.8,

                    CorPadraoOuro =
                        CorTriagem.Laranja,

                    DiscriminadoresEsperados =
                        "Dor abdominal intensa"
                },

                // =================================================
                // AMARELO
                // =================================================

                new CenarioClinico
                {
                    Titulo =
                        "Palpitação",

                    QueixaPrincipal =
                        "Palpitação",

                    Sintomas =
                        "Sensação de coração acelerado, sem dor torácica.",

                    TempoEvolucao =
                        "Há 1 hora",

                    FrequenciaCardiaca = 110,
                    FrequenciaRespiratoria = 18,
                    PressaoSistolica = 125,
                    Saturacao = 98,
                    Temperatura = 36.7,

                    CorPadraoOuro =
                        CorTriagem.Amarelo,

                    DiscriminadoresEsperados =
                        "Palpitação"
                },

                new CenarioClinico
                {
                    Titulo =
                        "Dor moderada",

                    QueixaPrincipal =
                        "Dor moderada",

                    Sintomas =
                        "Dor persistente sem sinais de instabilidade.",

                    TempoEvolucao =
                        "Há algumas horas",

                    FrequenciaCardiaca = 95,
                    FrequenciaRespiratoria = 18,
                    PressaoSistolica = 125,
                    Saturacao = 98,
                    Temperatura = 36.8,

                    CorPadraoOuro =
                        CorTriagem.Amarelo,

                    DiscriminadoresEsperados =
                        "Dor moderada"
                },

                new CenarioClinico
                {
                    Titulo =
                        "Dor abdominal moderada",

                    QueixaPrincipal =
                        "Dor abdominal",

                    Sintomas =
                        "Dor abdominal moderada sem sinais de instabilidade.",

                    TempoEvolucao =
                        "Há 4 horas",

                    FrequenciaCardiaca = 98,
                    FrequenciaRespiratoria = 18,
                    PressaoSistolica = 120,
                    Saturacao = 98,
                    Temperatura = 37.1,

                    CorPadraoOuro =
                        CorTriagem.Amarelo,

                    DiscriminadoresEsperados =
                        "Dor abdominal"
                },

                // =================================================
                // VERDE
                // =================================================

                new CenarioClinico
                {
                    Titulo =
                        "Cefaleia sem sinais de gravidade",

                    QueixaPrincipal =
                        "Dor de cabeça",

                    Sintomas =
                        "Cefaleia leve sem alteração de consciência.",

                    TempoEvolucao =
                        "Há 4 horas",

                    FrequenciaCardiaca = 82,
                    FrequenciaRespiratoria = 16,
                    PressaoSistolica = 120,
                    Saturacao = 98,
                    Temperatura = 36.5,

                    CorPadraoOuro =
                        CorTriagem.Verde,

                    DiscriminadoresEsperados =
                        "Cefaleia"
                },

                new CenarioClinico
                {
                    Titulo =
                        "Vômitos estáveis",

                    QueixaPrincipal =
                        "Vômito",

                    Sintomas =
                        "Episódios de vômito sem sinais de instabilidade.",

                    TempoEvolucao =
                        "Há 3 horas",

                    FrequenciaCardiaca = 88,
                    FrequenciaRespiratoria = 17,
                    PressaoSistolica = 118,
                    Saturacao = 98,
                    Temperatura = 36.7,

                    CorPadraoOuro =
                        CorTriagem.Verde,

                    DiscriminadoresEsperados =
                        "Vômitos"
                },

                new CenarioClinico
                {
                    Titulo =
                        "Náusea",

                    QueixaPrincipal =
                        "Náusea",

                    Sintomas =
                        "Náusea sem sinais de instabilidade.",

                    TempoEvolucao =
                        "Há 2 horas",

                    FrequenciaCardiaca = 80,
                    FrequenciaRespiratoria = 16,
                    PressaoSistolica = 120,
                    Saturacao = 98,
                    Temperatura = 36.5,

                    CorPadraoOuro =
                        CorTriagem.Verde,

                    DiscriminadoresEsperados =
                        "Náusea"
                },

                // =================================================
                // AZUL
                // =================================================

                new CenarioClinico
                {
                    Titulo =
                        "Renovação de receita",

                    QueixaPrincipal =
                        "Renovação de receita",

                    Sintomas =
                        "Solicitação administrativa sem sintomas agudos.",

                    TempoEvolucao =
                        "Demanda administrativa",

                    FrequenciaCardiaca = 75,
                    FrequenciaRespiratoria = 16,
                    PressaoSistolica = 120,
                    Saturacao = 98,
                    Temperatura = 36.5,

                    CorPadraoOuro =
                        CorTriagem.Azul,

                    DiscriminadoresEsperados =
                        "Renovação de receita"
                },

                new CenarioClinico
                {
                    Titulo =
                        "Pedido de exame",

                    QueixaPrincipal =
                        "Pedido de exame",

                    Sintomas =
                        "Paciente solicita pedido de exame simples.",

                    TempoEvolucao =
                        "Demanda administrativa",

                    FrequenciaCardiaca = 76,
                    FrequenciaRespiratoria = 16,
                    PressaoSistolica = 118,
                    Saturacao = 98,
                    Temperatura = 36.5,

                    CorPadraoOuro =
                        CorTriagem.Azul,

                    DiscriminadoresEsperados =
                        "Pedido de exame"
                }

            };

            // =====================================================
            // ADICIONA SOMENTE OS CENÁRIOS QUE AINDA NÃO EXISTEM
            // =====================================================

            foreach (var cenario in cenarios)
            {
                bool existe =
                    await context.CenariosClinicos
                        .AnyAsync(c =>
                            c.Titulo ==
                            cenario.Titulo);

                if (!existe)
                {
                    await context.CenariosClinicos
                        .AddAsync(cenario);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}