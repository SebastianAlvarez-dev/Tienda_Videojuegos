using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "juegos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    genero = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    precio = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    stock = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_juegos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    juego_id = table.Column<Guid>(type: "uuid", nullable: false),
                    precio = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    fecha_creacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ventas", x => x.id);
                    table.ForeignKey(
                        name: "FK_ventas_juegos_juego_id",
                        column: x => x.juego_id,
                        principalTable: "juegos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_juegos_titulo",
                table: "juegos",
                column: "titulo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ventas_juego_id",
                table: "ventas",
                column: "juego_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ventas");

            migrationBuilder.DropTable(
                name: "juegos");
        }
    }
}
