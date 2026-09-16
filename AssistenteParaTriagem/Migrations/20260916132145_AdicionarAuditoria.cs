using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenteParaTriagem.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    NomeProfissional = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Queixa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Sintomas = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    FrequenciaCardiaca = table.Column<int>(type: "INTEGER", nullable: true),
                    FrequenciaRespiratoria = table.Column<int>(type: "INTEGER", nullable: true),
                    PressaoSistolica = table.Column<int>(type: "INTEGER", nullable: true),
                    Saturacao = table.Column<int>(type: "INTEGER", nullable: true),
                    Temperatura = table.Column<double>(type: "REAL", nullable: true),
                    PacienteInconsciente = table.Column<bool>(type: "INTEGER", nullable: false),
                    Discriminadores = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    RegrasAplicadas = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    CorRisco = table.Column<int>(type: "INTEGER", nullable: false),
                    TempoMaximo = table.Column<int>(type: "INTEGER", nullable: false),
                    Justificativa = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    TipoOperacao = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DecisaoConcluida = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CorRisco",
                table: "AuditLogs",
                column: "CorRisco");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_DataHora",
                table: "AuditLogs",
                column: "DataHora");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");
        }
    }
}
