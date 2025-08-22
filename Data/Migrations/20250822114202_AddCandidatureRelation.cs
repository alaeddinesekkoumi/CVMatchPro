using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMatchPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidatureRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OffreEmploiId1",
                table: "Candidatures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_OffreEmploiId1",
                table: "Candidatures",
                column: "OffreEmploiId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidatures_OffresEmploi_OffreEmploiId1",
                table: "Candidatures",
                column: "OffreEmploiId1",
                principalTable: "OffresEmploi",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidatures_OffresEmploi_OffreEmploiId1",
                table: "Candidatures");

            migrationBuilder.DropIndex(
                name: "IX_Candidatures_OffreEmploiId1",
                table: "Candidatures");

            migrationBuilder.DropColumn(
                name: "OffreEmploiId1",
                table: "Candidatures");
        }
    }
}
