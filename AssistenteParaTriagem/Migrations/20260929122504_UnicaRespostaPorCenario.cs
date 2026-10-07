using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenteParaTriagem.Migrations
{
    /// <inheritdoc />
    public partial class UnicaRespostaPorCenario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AvaliacoesCenarios_CenarioClinicoId",
                table: "AvaliacoesCenarios");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesCenarios_CenarioClinicoId_NomeProfissional",
                table: "AvaliacoesCenarios",
                columns: new[] { "CenarioClinicoId", "NomeProfissional" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AvaliacoesCenarios_CenarioClinicoId_NomeProfissional",
                table: "AvaliacoesCenarios");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesCenarios_CenarioClinicoId",
                table: "AvaliacoesCenarios",
                column: "CenarioClinicoId");
        }
    }
}
