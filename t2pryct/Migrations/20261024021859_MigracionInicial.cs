using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace t2pryct.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    EntidadId = table.Column<int>(type: "int", nullable: true),
                    Detalle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Distritos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Provincia = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distritos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modalidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modalidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rubros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rubros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DistritoId = table.Column<int>(type: "int", nullable: true),
                    RubroId = table.Column<int>(type: "int", nullable: true),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alertas_Distritos_DistritoId",
                        column: x => x.DistritoId,
                        principalTable: "Distritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alertas_Rubros_RubroId",
                        column: x => x.RubroId,
                        principalTable: "Rubros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clusters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NombreRed = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DistritoId = table.Column<int>(type: "int", nullable: false),
                    RubroId = table.Column<int>(type: "int", nullable: false),
                    Severidad = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FiscalAsignado = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clusters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clusters_Distritos_DistritoId",
                        column: x => x.DistritoId,
                        principalTable: "Distritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clusters_Rubros_RubroId",
                        column: x => x.RubroId,
                        principalTable: "Rubros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Denunciados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Titular = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClusterId = table.Column<int>(type: "int", nullable: true),
                    FechaInclusionCluster = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denunciados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Denunciados_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "Clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModalidadExtorsionId = table.Column<int>(type: "int", nullable: false),
                    DistritoId = table.Column<int>(type: "int", nullable: false),
                    RubroId = table.Column<int>(type: "int", nullable: false),
                    NumeroEmisor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MedioPago = table.Column<int>(type: "int", nullable: false),
                    CuentaDestino = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Mensaje = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MontoExigido = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IpHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    EstadoVerificacion = table.Column<int>(type: "int", nullable: false),
                    ClusterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reportes_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "Clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reportes_Distritos_DistritoId",
                        column: x => x.DistritoId,
                        principalTable: "Distritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reportes_Modalidades_ModalidadExtorsionId",
                        column: x => x.ModalidadExtorsionId,
                        principalTable: "Modalidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reportes_Rubros_RubroId",
                        column: x => x.RubroId,
                        principalTable: "Rubros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PerfilesRiesgo",
                columns: table => new
                {
                    DenunciadoId = table.Column<int>(type: "int", nullable: false),
                    Puntaje = table.Column<int>(type: "int", nullable: false),
                    Semaforo = table.Column<int>(type: "int", nullable: false),
                    TotalReportes = table.Column<int>(type: "int", nullable: false),
                    TotalDistritos = table.Column<int>(type: "int", nullable: false),
                    FechaCalculo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilesRiesgo", x => x.DenunciadoId);
                    table.ForeignKey(
                        name: "FK_PerfilesRiesgo_Denunciados_DenunciadoId",
                        column: x => x.DenunciadoId,
                        principalTable: "Denunciados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evidencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReporteId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evidencias_Reportes_ReporteId",
                        column: x => x.ReporteId,
                        principalTable: "Reportes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteDenunciados",
                columns: table => new
                {
                    ReporteId = table.Column<int>(type: "int", nullable: false),
                    DenunciadoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteDenunciados", x => new { x.ReporteId, x.DenunciadoId });
                    table.ForeignKey(
                        name: "FK_ReporteDenunciados_Denunciados_DenunciadoId",
                        column: x => x.DenunciadoId,
                        principalTable: "Denunciados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReporteDenunciados_Reportes_ReporteId",
                        column: x => x.ReporteId,
                        principalTable: "Reportes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Denunciados",
                columns: new[] { "Id", "ClusterId", "FechaInclusionCluster", "FechaRegistro", "Tipo", "Titular", "Valor" },
                values: new object[,]
                {
                    { 4, null, null, new DateTime(2026, 9, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), 1, null, "945123789" },
                    { 5, null, null, new DateTime(2026, 9, 14, 20, 0, 0, 0, DateTimeKind.Unspecified), 1, null, "933221100" }
                });

            migrationBuilder.InsertData(
                table: "Distritos",
                columns: new[] { "Id", "Activo", "Nombre", "Provincia" },
                values: new object[,]
                {
                    { 1, true, "Trujillo", "Trujillo" },
                    { 2, true, "El Porvenir", "Trujillo" },
                    { 3, true, "La Esperanza", "Trujillo" },
                    { 4, true, "Florencia de Mora", "Trujillo" },
                    { 5, true, "Alto Trujillo", "Trujillo" },
                    { 6, true, "Víctor Larco Herrera", "Trujillo" }
                });

            migrationBuilder.InsertData(
                table: "Modalidades",
                columns: new[] { "Id", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Llamada amenazante" },
                    { 2, true, "Mensaje de WhatsApp" },
                    { 3, true, "Cobro de cupo presencial" },
                    { 4, true, "Préstamo gota a gota" },
                    { 5, true, "Atentado con explosivo" }
                });

            migrationBuilder.InsertData(
                table: "Rubros",
                columns: new[] { "Id", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Bodega" },
                    { 2, true, "Transporte" },
                    { 3, true, "Calzado" },
                    { 4, true, "Farmacia" },
                    { 5, true, "Mercado" },
                    { 6, true, "Otro" }
                });

            migrationBuilder.InsertData(
                table: "Alertas",
                columns: new[] { "Id", "Activa", "DistritoId", "FechaPublicacion", "Mensaje", "RubroId", "Titulo" },
                values: new object[] { 1, true, 1, new DateTime(2026, 9, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), "Se detectaron mensajes que amenazan con denunciar falsamente al comerciante si no deposita dinero. No pague, bloquee el número y reporte en la plataforma.", 4, "Nueva modalidad: falsa denuncia como amenaza" });

            migrationBuilder.InsertData(
                table: "Clusters",
                columns: new[] { "Id", "Codigo", "DistritoId", "Estado", "FechaCreacion", "FiscalAsignado", "NombreRed", "Observaciones", "RubroId", "Severidad" },
                values: new object[] { 1, "EXP-2026-TRU-0001", 2, 1, new DateTime(2026, 9, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), "Fiscal Provincial de Turno - Trujillo", "Red Extorsiva El Porvenir - Microempresarios del Calzado", "Mensajes con plantilla similar y mismo número emisor en tres distritos.", 3, 2 });

            migrationBuilder.InsertData(
                table: "PerfilesRiesgo",
                columns: new[] { "DenunciadoId", "FechaCalculo", "Puntaje", "Semaforo", "TotalDistritos", "TotalReportes" },
                values: new object[,]
                {
                    { 4, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 45, 3, 2, 2 },
                    { 5, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 25, 2, 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "Reportes",
                columns: new[] { "Id", "ClusterId", "Codigo", "CuentaDestino", "DistritoId", "EstadoVerificacion", "Fecha", "IpHash", "MedioPago", "Mensaje", "ModalidadExtorsionId", "MontoExigido", "NumeroEmisor", "RubroId" },
                values: new object[,]
                {
                    { 5, null, "REP-20260912-0005", null, 5, 1, new DateTime(2026, 9, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", 2, "Entrega la cuota semanal de 200 soles o atacamos tu unidad de transporte", 4, 200m, "945123789", 2 },
                    { 6, null, "REP-20260913-0006", null, 2, 1, new DateTime(2026, 9, 13, 11, 40, 0, 0, DateTimeKind.Unspecified), "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", 2, "Entrega la cuota semanal de 150 soles o atacamos tu bodega", 1, 150m, "945123789", 1 },
                    { 7, null, "REP-20260914-0007", null, 1, 1, new DateTime(2026, 9, 14, 20, 0, 0, 0, DateTimeKind.Unspecified), "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", 5, "Si no depositas 100 soles te denunciaremos falsamente ante la policia", 2, 100m, "933221100", 4 }
                });

            migrationBuilder.InsertData(
                table: "Denunciados",
                columns: new[] { "Id", "ClusterId", "FechaInclusionCluster", "FechaRegistro", "Tipo", "Titular", "Valor" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, "Desconocido", "987654321" },
                    { 2, 1, new DateTime(2026, 9, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, "J. R. M.", "912345678" },
                    { 3, 1, new DateTime(2026, 9, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 3, 15, 0, 0, 0, DateTimeKind.Unspecified), 4, "Cuenta puente", "19112345678901" }
                });

            migrationBuilder.InsertData(
                table: "Evidencias",
                columns: new[] { "Id", "Descripcion", "FechaRegistro", "ReporteId" },
                values: new object[] { 3, "Captura del mensaje de texto recibido", new DateTime(2026, 9, 14, 20, 5, 0, 0, DateTimeKind.Unspecified), 7 });

            migrationBuilder.InsertData(
                table: "ReporteDenunciados",
                columns: new[] { "DenunciadoId", "ReporteId" },
                values: new object[,]
                {
                    { 4, 5 },
                    { 4, 6 },
                    { 5, 7 }
                });

            migrationBuilder.InsertData(
                table: "Reportes",
                columns: new[] { "Id", "ClusterId", "Codigo", "CuentaDestino", "DistritoId", "EstadoVerificacion", "Fecha", "IpHash", "MedioPago", "Mensaje", "ModalidadExtorsionId", "MontoExigido", "NumeroEmisor", "RubroId" },
                values: new object[,]
                {
                    { 1, 1, "REP-20260901-0001", "912345678", 2, 2, new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", 1, "Paga 500 soles hoy o volamos tu taller con dinamita. Sabemos donde vives", 2, 500m, "987654321", 3 },
                    { 2, 1, "REP-20260903-0002", "19112345678901", 4, 2, new DateTime(2026, 9, 3, 15, 0, 0, 0, DateTimeKind.Unspecified), "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", 3, "Paga 500 soles hoy o volamos tu taller con dinamita. Sabemos donde trabajas", 1, 500m, "987654321", 3 },
                    { 3, 1, "REP-20260905-0003", null, 2, 1, new DateTime(2026, 9, 5, 18, 30, 0, 0, DateTimeKind.Unspecified), "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", 1, "Deposita 300 soles hoy a este yape o te quemamos la fabrica", 2, 300m, "912345678", 3 },
                    { 4, 1, "REP-20260908-0004", null, 3, 1, new DateTime(2026, 9, 8, 9, 15, 0, 0, DateTimeKind.Unspecified), "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", 1, "Paga 300 soles hoy o volamos tu bodega con dinamita. Sabemos donde vives", 2, 300m, "987654321", 1 }
                });

            migrationBuilder.InsertData(
                table: "Evidencias",
                columns: new[] { "Id", "Descripcion", "FechaRegistro", "ReporteId" },
                values: new object[,]
                {
                    { 1, "Captura del chat de WhatsApp con la amenaza", new DateTime(2026, 9, 1, 10, 5, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Captura del voucher de transferencia exigida", new DateTime(2026, 9, 3, 15, 5, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.InsertData(
                table: "PerfilesRiesgo",
                columns: new[] { "DenunciadoId", "FechaCalculo", "Puntaje", "Semaforo", "TotalDistritos", "TotalReportes" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 100, 3, 3, 3 },
                    { 2, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 80, 3, 1, 2 },
                    { 3, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 50, 3, 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "ReporteDenunciados",
                columns: new[] { "DenunciadoId", "ReporteId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 1, 2 },
                    { 3, 2 },
                    { 2, 3 },
                    { 1, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_DistritoId",
                table: "Alertas",
                column: "DistritoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_RubroId",
                table: "Alertas",
                column: "RubroId");

            migrationBuilder.CreateIndex(
                name: "IX_Clusters_Codigo",
                table: "Clusters",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clusters_DistritoId",
                table: "Clusters",
                column: "DistritoId");

            migrationBuilder.CreateIndex(
                name: "IX_Clusters_RubroId",
                table: "Clusters",
                column: "RubroId");

            migrationBuilder.CreateIndex(
                name: "IX_Denunciados_ClusterId",
                table: "Denunciados",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_Denunciados_Valor",
                table: "Denunciados",
                column: "Valor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evidencias_ReporteId",
                table: "Evidencias",
                column: "ReporteId");

            migrationBuilder.CreateIndex(
                name: "IX_ReporteDenunciados_DenunciadoId",
                table: "ReporteDenunciados",
                column: "DenunciadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_ClusterId",
                table: "Reportes",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_Codigo",
                table: "Reportes",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_DistritoId",
                table: "Reportes",
                column: "DistritoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_ModalidadExtorsionId",
                table: "Reportes",
                column: "ModalidadExtorsionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_RubroId",
                table: "Reportes",
                column: "RubroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Evidencias");

            migrationBuilder.DropTable(
                name: "PerfilesRiesgo");

            migrationBuilder.DropTable(
                name: "ReporteDenunciados");

            migrationBuilder.DropTable(
                name: "Denunciados");

            migrationBuilder.DropTable(
                name: "Reportes");

            migrationBuilder.DropTable(
                name: "Clusters");

            migrationBuilder.DropTable(
                name: "Modalidades");

            migrationBuilder.DropTable(
                name: "Distritos");

            migrationBuilder.DropTable(
                name: "Rubros");
        }
    }
}
