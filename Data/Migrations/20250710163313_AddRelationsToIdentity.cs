using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMatchPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationsToIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pays",
                table: "Entreprises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Entreprises",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Candidats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Entreprises_UserId",
                table: "Entreprises",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidats_UserId",
                table: "Candidats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidats_AspNetUsers_UserId",
                table: "Candidats",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Entreprises_AspNetUsers_UserId",
                table: "Entreprises",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidats_AspNetUsers_UserId",
                table: "Candidats");

            migrationBuilder.DropForeignKey(
                name: "FK_Entreprises_AspNetUsers_UserId",
                table: "Entreprises");

            migrationBuilder.DropIndex(
                name: "IX_Entreprises_UserId",
                table: "Entreprises");

            migrationBuilder.DropIndex(
                name: "IX_Candidats_UserId",
                table: "Candidats");

            migrationBuilder.DropColumn(
                name: "Pays",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Candidats");
        }
    }
}
