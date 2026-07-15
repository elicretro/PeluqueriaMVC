using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeluqueriaCanina.Migrations
{
    /// <inheritdoc />
    public partial class RelacionVentaItemCarrito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsCarrito_Venta_VentaId",
                table: "ItemsCarrito");

            migrationBuilder.DropForeignKey(
                name: "FK_Venta_Personas_ClienteId",
                table: "Venta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Venta",
                table: "Venta");

            migrationBuilder.RenameTable(
                name: "Venta",
                newName: "Ventas");

            migrationBuilder.RenameIndex(
                name: "IX_Venta_ClienteId",
                table: "Ventas",
                newName: "IX_Ventas_ClienteId");

            migrationBuilder.AlterColumn<int>(
                name: "VentaId",
                table: "ItemsCarrito",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ventas",
                table: "Ventas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsCarrito_Ventas_VentaId",
                table: "ItemsCarrito",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Personas_ClienteId",
                table: "Ventas",
                column: "ClienteId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsCarrito_Ventas_VentaId",
                table: "ItemsCarrito");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Personas_ClienteId",
                table: "Ventas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ventas",
                table: "Ventas");

            migrationBuilder.RenameTable(
                name: "Ventas",
                newName: "Venta");

            migrationBuilder.RenameIndex(
                name: "IX_Ventas_ClienteId",
                table: "Venta",
                newName: "IX_Venta_ClienteId");

            migrationBuilder.AlterColumn<int>(
                name: "VentaId",
                table: "ItemsCarrito",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Venta",
                table: "Venta",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsCarrito_Venta_VentaId",
                table: "ItemsCarrito",
                column: "VentaId",
                principalTable: "Venta",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Venta_Personas_ClienteId",
                table: "Venta",
                column: "ClienteId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
