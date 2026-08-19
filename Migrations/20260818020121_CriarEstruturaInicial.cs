using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransparenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class CriarEstruturaInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TelefoneHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CriadoEmUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mensagens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentificadorWhatsApp = table.Column<string>(type: "text", nullable: false),
                    Conteudo = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Estado = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    RecebidaEmUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensagens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mensagens_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FalhasProcessamento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MensagemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Etapa = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Detalhe = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    RegistradaEmUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FalhasProcessamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FalhasProcessamento_Mensagens_MensagemId",
                        column: x => x.MensagemId,
                        principalTable: "Mensagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FalhasProcessamento_MensagemId",
                table: "FalhasProcessamento",
                column: "MensagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensagens_IdentificadorWhatsApp",
                table: "Mensagens",
                column: "IdentificadorWhatsApp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mensagens_UsuarioId",
                table: "Mensagens",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_TelefoneHash",
                table: "Usuarios",
                column: "TelefoneHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FalhasProcessamento");

            migrationBuilder.DropTable(
                name: "Mensagens");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
