using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMatchPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "Candidats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Candidats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Candidats",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "Candidats");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Candidats");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Candidats");
        }
    }
}
