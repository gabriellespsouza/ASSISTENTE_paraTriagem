using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenteParaTriagem.Migrations
{
    /// <inheritdoc />
    public partial class RemoverAvaliacaoTriagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliacoes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avaliacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CorRisco = table.Column<int>(type: "INTEGER", nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Discriminadores = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    FrequenciaCardiaca = table.Column<int>(type: "INTEGER", nullable: true),
                    FrequenciaRespiratoria = table.Column<int>(type: "INTEGER", nullable: true),
                    Justificativa = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    NomeProfissional = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PacienteInconsciente = table.Column<bool>(type: "INTEGER", nullable: false),
                    PressaoSistolica = table.Column<int>(type: "INTEGER", nullable: true),
                    Queixa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    RegrasAplicadas = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Saturacao = table.Column<int>(type: "INTEGER", nullable: true),
                    Sintomas = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Temperatura = table.Column<double>(type: "REAL", nullable: true),
                    TempoMaximo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacoes", x => x.Id);
                });
        }
    }
}
