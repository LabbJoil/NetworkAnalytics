using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NetworkAnalytics.Migrations
{
    /// <inheritdoc />
    public partial class NetworkAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Login = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SecondName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUser = table.Column<int>(type: "integer", nullable: false),
                    SocialNetwork = table.Column<string>(type: "text", nullable: false),
                    Login = table.Column<string>(type: "text", nullable: false),
                    Dialog = table.Column<string>(type: "text", nullable: false),
                    CountMessages = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.Id);
                    table.ForeignKey(
                        "FK_reportuser", x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade,
                        onUpdate: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommonWord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdReport = table.Column<int>(type: "integer", nullable: false),
                    Word = table.Column<string>(type: "text", nullable: false),
                    NumberRepetitions = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonWord", x => x.Id);
                    table.ForeignKey(
                    "FK_commonword", x => x.IdReport,
                    principalTable: "Report",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade,
                    onUpdate: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartsSpeech",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdReport = table.Column<int>(type: "integer", nullable: false),
                    NOUN = table.Column<int>(type: "integer", nullable: false),
                    DET = table.Column<int>(type: "integer", nullable: false),
                    ADJ = table.Column<int>(type: "integer", nullable: false),
                    PART = table.Column<int>(type: "integer", nullable: false),
                    PRON = table.Column<int>(type: "integer", nullable: false),
                    ADP = table.Column<int>(type: "integer", nullable: false),
                    VERB = table.Column<int>(type: "integer", nullable: false),
                    NUM = table.Column<int>(type: "integer", nullable: false),
                    ADV = table.Column<int>(type: "integer", nullable: false),
                    INTJ = table.Column<int>(type: "integer", nullable: false),
                    SYM = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartsSpeech", x => x.Id);
                    table.ForeignKey(
                    "FK_partsspeech", x => x.IdReport,
                    principalTable: "Report",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade,
                    onUpdate: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelegramUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUser = table.Column<int>(type: "integer", nullable: false),
                    NickName = table.Column<string>(type: "text", nullable: true),
                    Login = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<byte[]>(type: "bytea", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    NameSession = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUser", x => x.Id);
                    table.ForeignKey(
                        "FK_telegramuser", x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade,
                        onUpdate: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Them",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdReport = table.Column<int>(type: "integer", nullable: false),
                    Sport = table.Column<float>(type: "real", nullable: false),
                    Politics = table.Column<float>(type: "real", nullable: false),
                    Art = table.Column<float>(type: "real", nullable: false),
                    Technic = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Them", x => x.Id);
                    table.ForeignKey(
                     "FK_them", x => x.IdReport,
                     principalTable: "Report",
                     principalColumn: "Id",
                     onDelete: ReferentialAction.Cascade,
                     onUpdate: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tonality",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdReport = table.Column<int>(type: "integer", nullable: false),
                    Positive = table.Column<float>(type: "real", nullable: false),
                    Median = table.Column<float>(type: "real", nullable: false),
                    Negative = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tonality", x => x.Id);
                    table.ForeignKey(
                    "FK_tonality", x => x.IdReport,
                    principalTable: "Report",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade,
                    onUpdate: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUser = table.Column<int>(type: "integer", nullable: false),
                    NickName = table.Column<string>(type: "text", nullable: true),
                    Login = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<byte[]>(type: "bytea", nullable: true),
                    VkToken = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkUser", x => x.Id);
                    table.ForeignKey(
                        "FK_vkuser", x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade,
                        onUpdate: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommonWord");

            migrationBuilder.DropTable(
                name: "PartsSpeech");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "TelegramUser");

            migrationBuilder.DropTable(
                name: "Them");

            migrationBuilder.DropTable(
                name: "Tonality");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "VkUser");
        }
    }
}
