using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryBackend.Migrations
{
    /// <inheritdoc />
    public partial class DishRatingAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Dishes",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Dishes");
        }
    }
}
