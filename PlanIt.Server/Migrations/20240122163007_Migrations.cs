using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PII.Migrations
{
    public partial class Migrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.UtilisateurId);
                });

            migrationBuilder.CreateTable(
                name: "Disponibilite",
                columns: table => new
                {
                    DisponibiliteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NbHeure = table.Column<int>(type: "INTEGER", nullable: false),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disponibilite", x => x.DisponibiliteId);
                    table.ForeignKey(
                        name: "FK_Disponibilite_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "UtilisateurId");
                });

            migrationBuilder.CreateTable(
                name: "Taches",
                columns: table => new
                {
                    TacheId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Duree = table.Column<int>(type: "INTEGER", nullable: false),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taches", x => x.TacheId);
                    table.ForeignKey(
                        name: "FK_Taches_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "UtilisateurId");
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    TodoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Duree = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    TacheId = table.Column<int>(type: "INTEGER", nullable: false),
                    Realisation = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.TodoId);
                    table.ForeignKey(
                        name: "FK_Todos_Taches_TacheId",
                        column: x => x.TacheId,
                        principalTable: "Taches",
                        principalColumn: "TacheId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Todos_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "UtilisateurId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disponibilite_UtilisateurId",
                table: "Disponibilite",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Taches_UtilisateurId",
                table: "Taches",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_TacheId",
                table: "Todos",
                column: "TacheId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_UtilisateurId",
                table: "Todos",
                column: "UtilisateurId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disponibilite");

            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.DropTable(
                name: "Taches");

            migrationBuilder.DropTable(
                name: "Utilisateurs");
        }
    }
}
