using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace t2pryct.Migrations
{
    /// <inheritdoc />
    public partial class AgregarArchivosEvidencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombreArchivo",
                table: "Evidencias",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RutaArchivo",
                table: "Evidencias",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Evidencias",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "NombreArchivo", "RutaArchivo" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Evidencias",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "NombreArchivo", "RutaArchivo" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Evidencias",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "NombreArchivo", "RutaArchivo" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreArchivo",
                table: "Evidencias");

            migrationBuilder.DropColumn(
                name: "RutaArchivo",
                table: "Evidencias");
        }
    }
}
