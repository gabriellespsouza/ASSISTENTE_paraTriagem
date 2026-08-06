using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenteParaTriagem.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarColunaRegrasAplicadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegrasAplicadas",
                table: "Avaliacoes",
                type: "TEXT",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegrasAplicadas",
                table: "Avaliacoes");
        }
    }
}
