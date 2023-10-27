using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentScheduler.Migrations
{
    /// <inheritdoc />
    public partial class FloatPointValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "VictoryPoints",
                table: "Competitions",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<float>(
                name: "DrawPoints",
                table: "Competitions",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<float>(
                name: "DefeatPoints",
                table: "Competitions",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VictoryPoints",
                table: "Competitions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "DrawPoints",
                table: "Competitions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "DefeatPoints",
                table: "Competitions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
