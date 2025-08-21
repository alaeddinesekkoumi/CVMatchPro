using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMatchPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEntrepriseDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "Domaine",
                table: "Entreprises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Entreprises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ville",
                table: "Entreprises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Candidats",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domaine",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "Ville",
                table: "Entreprises");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Candidats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidatId = table.Column<int>(type: "int", nullable: false),
                    EntrepriseId = table.Column<int>(type: "int", nullable: false),
                    OffreEmploiId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Candidats_CandidatId",
                        column: x => x.CandidatId,
                        principalTable: "Candidats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Entreprises_EntrepriseId",
                        column: x => x.EntrepriseId,
                        principalTable: "Entreprises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_OffresEmploi_OffreEmploiId",
                        column: x => x.OffreEmploiId,
                        principalTable: "OffresEmploi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CandidatId",
                table: "Notifications",
                column: "CandidatId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EntrepriseId",
                table: "Notifications",
                column: "EntrepriseId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_OffreEmploiId",
                table: "Notifications",
                column: "OffreEmploiId");
        }
    }
}
