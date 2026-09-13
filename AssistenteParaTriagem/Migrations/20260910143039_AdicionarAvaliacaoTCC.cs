using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenteParaTriagem.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarAvaliacaoTCC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CenariosClinicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    QueixaPrincipal = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Sintomas = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    TempoEvolucao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    FrequenciaCardiaca = table.Column<int>(type: "INTEGER", nullable: true),
                    FrequenciaRespiratoria = table.Column<int>(type: "INTEGER", nullable: true),
                    PressaoSistolica = table.Column<int>(type: "INTEGER", nullable: true),
                    Saturacao = table.Column<int>(type: "INTEGER", nullable: true),
                    Temperatura = table.Column<double>(type: "REAL", nullable: true),
                    PacienteInconsciente = table.Column<bool>(type: "INTEGER", nullable: false),
                    DiscriminadoresEsperados = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CorPadraoOuro = table.Column<int>(type: "INTEGER", nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenariosClinicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RespostasQuestionarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeProfissional = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    FacilidadeUso = table.Column<int>(type: "INTEGER", nullable: false),
                    ClarezaRecomendacao = table.Column<int>(type: "INTEGER", nullable: false),
                    Utilidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Confianca = table.Column<int>(type: "INTEGER", nullable: false),
                    RecomendariaUso = table.Column<int>(type: "INTEGER", nullable: false),
                    PontosPositivos = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Dificuldades = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Sugestoes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespostasQuestionarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AvaliacoesCenarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CenarioClinicoId = table.Column<int>(type: "INTEGER", nullable: false),
                    NomeProfissional = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    CorProfissional = table.Column<int>(type: "INTEGER", nullable: false),
                    CorSistema = table.Column<int>(type: "INTEGER", nullable: false),
                    CorPadraoOuro = table.Column<int>(type: "INTEGER", nullable: false),
                    SistemaAcertou = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProfissionalAcertou = table.Column<bool>(type: "INTEGER", nullable: false),
                    Subtriagem = table.Column<bool>(type: "INTEGER", nullable: false),
                    Supertriagem = table.Column<bool>(type: "INTEGER", nullable: false),
                    DiscriminadoresIdentificados = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    RegrasAplicadas = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    JustificativaSistema = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacoesCenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvaliacoesCenarios_CenariosClinicos_CenarioClinicoId",
                        column: x => x.CenarioClinicoId,
                        principalTable: "CenariosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesCenarios_CenarioClinicoId",
                table: "AvaliacoesCenarios",
                column: "CenarioClinicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvaliacoesCenarios");

            migrationBuilder.DropTable(
                name: "RespostasQuestionarios");

            migrationBuilder.DropTable(
                name: "CenariosClinicos");
        }
    }
}
