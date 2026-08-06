using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenteParaTriagem.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDiscriminadores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminadores",
                table: "Avaliacoes",
                type: "TEXT",
                maxLength: 3000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FrequenciaCardiaca",
                table: "Avaliacoes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrequenciaRespiratoria",
                table: "Avaliacoes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PacienteInconsciente",
                table: "Avaliacoes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PressaoSistolica",
                table: "Avaliacoes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Queixa",
                table: "Avaliacoes",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Saturacao",
                table: "Avaliacoes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sintomas",
                table: "Avaliacoes",
                type: "TEXT",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Temperatura",
                table: "Avaliacoes",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminadores",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "FrequenciaCardiaca",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "FrequenciaRespiratoria",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "PacienteInconsciente",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "PressaoSistolica",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "Queixa",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "Saturacao",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "Sintomas",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "Temperatura",
                table: "Avaliacoes");
        }
    }
}
