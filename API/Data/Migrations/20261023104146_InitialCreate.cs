using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "devops_training");

            migrationBuilder.CreateTable(
                name: "t_e_user_use",
                schema: "devops_training",
                columns: table => new
                {
                    use_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    use_last_name = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    use_first_name = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    use_email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    use_password_hash = table.Column<string>(type: "text", nullable: false),
                    use_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    use_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_user_use", x => x.use_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_user_use_use_email",
                schema: "devops_training",
                table: "t_e_user_use",
                column: "use_email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_e_user_use",
                schema: "devops_training");
        }
    }
}
