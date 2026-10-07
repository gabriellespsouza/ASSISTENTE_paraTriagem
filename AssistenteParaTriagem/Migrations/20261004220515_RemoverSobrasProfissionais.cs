using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenteParaTriagem.Migrations
{
    /// <inheritdoc />
    public partial class RemoverSobrasProfissionais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespostasQuestionarios");

            migrationBuilder.DropColumn(
                name: "CorProfissional",
                table: "AvaliacoesCenarios");

            migrationBuilder.DropColumn(
                name: "ProfissionalAcertou",
                table: "AvaliacoesCenarios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CorProfissional",
                table: "AvaliacoesCenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ProfissionalAcertou",
                table: "AvaliacoesCenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RespostasQuestionarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClarezaRecomendacao = table.Column<int>(type: "INTEGER", nullable: false),
                    Confianca = table.Column<int>(type: "INTEGER", nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Dificuldades = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    FacilidadeUso = table.Column<int>(type: "INTEGER", nullable: false),
                    NomeProfissional = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PontosPositivos = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    RecomendariaUso = table.Column<int>(type: "INTEGER", nullable: false),
                    Sugestoes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Utilidade = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespostasQuestionarios", x => x.Id);
                });
        }
    }
}
