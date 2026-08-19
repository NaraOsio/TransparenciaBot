using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransparenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTabelaGastos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gastos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdDeputadoCamara = table.Column<int>(type: "integer", nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Mes = table.Column<int>(type: "integer", nullable: false),
                    TipoDespesa = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ValorDocumento = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorLiquido = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NomeFornecedor = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    DataDocumento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UrlDocumento = table.Column<string>(type: "text", nullable: true),
                    ImportadoEmUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_IdDeputadoCamara_Ano",
                table: "Gastos",
                columns: new[] { "IdDeputadoCamara", "Ano" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gastos");
        }
    }
}
