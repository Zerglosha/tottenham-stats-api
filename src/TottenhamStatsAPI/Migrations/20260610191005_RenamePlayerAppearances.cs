using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TottenhamStatsAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenamePlayerAppearances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "Apperances",
                table: "Players",
                newName: "Appearances");

            migrationBuilder.AlterColumn<int>(
                name: "ClubId",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "ClubId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "Appearances",
                table: "Players",
                newName: "Apperances");

            migrationBuilder.AlterColumn<int>(
                name: "ClubId",
                table: "Players",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "ClubId");
        }
    }
}
