using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureYouAI.Migrations
{
    /// <inheritdoc />
    public partial class mig14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PricingPlanItems",
                columns: table => new
                {
                    PricingPlanItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricingPlanId = table.Column<int>(type: "int", nullable: false),
                    PricingPlanItemId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingPlanItems", x => x.PricingPlanItemId);
                    table.ForeignKey(
                        name: "FK_PricingPlanItems_PricingPlanItems_PricingPlanItemId1",
                        column: x => x.PricingPlanItemId1,
                        principalTable: "PricingPlanItems",
                        principalColumn: "PricingPlanItemId");
                    table.ForeignKey(
                        name: "FK_PricingPlanItems_PricingPlans_PricingPlanId",
                        column: x => x.PricingPlanId,
                        principalTable: "PricingPlans",
                        principalColumn: "PricingPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricingPlanItems_PricingPlanId",
                table: "PricingPlanItems",
                column: "PricingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PricingPlanItems_PricingPlanItemId1",
                table: "PricingPlanItems",
                column: "PricingPlanItemId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingPlanItems");
        }
    }
}
