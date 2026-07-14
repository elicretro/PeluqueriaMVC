using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeluqueriaCanina.Migrations
{
    /// <inheritdoc />
    public partial class RelacionClienteMascotaFija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mascotas_Personas_ClienteId",
                table: "Mascotas");

            migrationBuilder.AddForeignKey(
                name: "FK_Mascotas_Personas_ClienteId",
                table: "Mascotas",
                column: "ClienteId",
                principalTable: "Personas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mascotas_Personas_ClienteId",
                table: "Mascotas");

            migrationBuilder.AddForeignKey(
                name: "FK_Mascotas_Personas_ClienteId",
                table: "Mascotas",
                column: "ClienteId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
