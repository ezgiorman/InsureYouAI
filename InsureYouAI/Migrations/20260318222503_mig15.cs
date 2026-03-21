using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureYouAI.Migrations
{
    /// <inheritdoc />
    public partial class mig15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricingPlanItems_PricingPlanItems_PricingPlanItemId1",
                table: "PricingPlanItems");

            migrationBuilder.DropIndex(
                name: "IX_PricingPlanItems_PricingPlanItemId1",
                table: "PricingPlanItems");

            migrationBuilder.DropColumn(
                name: "PricingPlanItemId1",
                table: "PricingPlanItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PricingPlanItemId1",
                table: "PricingPlanItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PricingPlanItems_PricingPlanItemId1",
                table: "PricingPlanItems",
                column: "PricingPlanItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PricingPlanItems_PricingPlanItems_PricingPlanItemId1",
                table: "PricingPlanItems",
                column: "PricingPlanItemId1",
                principalTable: "PricingPlanItems",
                principalColumn: "PricingPlanItemId");
        }
    }
}
