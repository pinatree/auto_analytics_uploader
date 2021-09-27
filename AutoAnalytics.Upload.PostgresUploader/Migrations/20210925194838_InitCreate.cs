using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AutoAnalytics.DbUpload.DataUpload.PostgreSQL.Migrations
{
    public partial class InitCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auto_analytics");

            migrationBuilder.CreateSequence(
                name: "t_group_id_sequence",
                schema: "auto_analytics",
                minValue: 0L,
                maxValue: 99999999999999999L);

            migrationBuilder.CreateTable(
                name: "t_assoc_rule",
                schema: "auto_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'', '1', '0', '9999999999', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    reason_detail_id = table.Column<long>(type: "bigint", nullable: false),
                    conseq_detail_id = table.Column<long>(type: "bigint", nullable: false),
                    c_calc_date = table.Column<DateTime>(type: "date", nullable: false),
                    c_support_count = table.Column<long>(type: "bigint", nullable: false),
                    c_support_perc = table.Column<decimal>(type: "numeric", nullable: false),
                    c_reliability = table.Column<decimal>(type: "numeric", nullable: false),
                    c_lift = table.Column<decimal>(type: "numeric", nullable: false),
                    region_id = table.Column<long>(type: "bigint", nullable: false),
                    model_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_assoc_rule", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_group",
                schema: "auto_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'', '1', '0', '9999999999', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    c_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_model",
                schema: "auto_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'', '1', '0', '9999999999', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    c_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_model", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_region",
                schema: "auto_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'', '1', '0', '9999999999', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    c_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_region", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_subgroup",
                schema: "auto_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'', '1', '0', '9999999999', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    group_id = table.Column<long>(type: "bigint", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_subgroup", x => x.id);
                    table.ForeignKey(
                        name: "group_for_subgroup",
                        column: x => x.group_id,
                        principalSchema: "auto_analytics",
                        principalTable: "t_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_detail",
                schema: "auto_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'', '1', '0', '9999999999', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    subgroup_id = table.Column<long>(type: "bigint", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_detail", x => x.id);
                    table.ForeignKey(
                        name: "subgroup_for_detail",
                        column: x => x.subgroup_id,
                        principalSchema: "auto_analytics",
                        principalTable: "t_subgroup",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_crash",
                schema: "auto_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'', '1', '0', '9999999999', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    crash_id = table.Column<string>(type: "text", nullable: false, comment: "ComplaintId + \" - \" + EngineNum"),
                    damage_detail_id = table.Column<long>(type: "bigint", nullable: false),
                    c_date = table.Column<DateTime>(type: "date", nullable: true),
                    c_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_crash", x => x.id);
                    table.ForeignKey(
                        name: "detail_dependency",
                        column: x => x.damage_detail_id,
                        principalSchema: "auto_analytics",
                        principalTable: "t_detail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_crash_damage_detail_id",
                schema: "auto_analytics",
                table: "t_crash",
                column: "damage_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_detail_subgroup_id",
                schema: "auto_analytics",
                table: "t_detail",
                column: "subgroup_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_subgroup_group_id",
                schema: "auto_analytics",
                table: "t_subgroup",
                column: "group_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_assoc_rule",
                schema: "auto_analytics");

            migrationBuilder.DropTable(
                name: "t_crash",
                schema: "auto_analytics");

            migrationBuilder.DropTable(
                name: "t_model",
                schema: "auto_analytics");

            migrationBuilder.DropTable(
                name: "t_region",
                schema: "auto_analytics");

            migrationBuilder.DropTable(
                name: "t_detail",
                schema: "auto_analytics");

            migrationBuilder.DropTable(
                name: "t_subgroup",
                schema: "auto_analytics");

            migrationBuilder.DropTable(
                name: "t_group",
                schema: "auto_analytics");

            migrationBuilder.DropSequence(
                name: "t_group_id_sequence",
                schema: "auto_analytics");
        }
    }
}
