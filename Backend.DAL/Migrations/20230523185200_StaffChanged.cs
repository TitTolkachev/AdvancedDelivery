using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StaffChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cook_Restaurants_RestaurantId",
                table: "Cook");

            migrationBuilder.DropForeignKey(
                name: "FK_Manager_Restaurants_RestaurantId",
                table: "Manager");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Cook_CookId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cook",
                table: "Cook");

            migrationBuilder.RenameTable(
                name: "Cook",
                newName: "Cooks");

            migrationBuilder.RenameIndex(
                name: "IX_Cook_RestaurantId",
                table: "Cooks",
                newName: "IX_Cooks_RestaurantId");

            migrationBuilder.AddColumn<Guid>(
                name: "CourierId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RestaurantId",
                table: "Manager",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RestaurantId",
                table: "Cooks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cooks",
                table: "Cooks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Courier",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courier", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CourierId",
                table: "Orders",
                column: "CourierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cooks_Restaurants_RestaurantId",
                table: "Cooks",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Manager_Restaurants_RestaurantId",
                table: "Manager",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Cooks_CookId",
                table: "Orders",
                column: "CookId",
                principalTable: "Cooks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Courier_CourierId",
                table: "Orders",
                column: "CourierId",
                principalTable: "Courier",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cooks_Restaurants_RestaurantId",
                table: "Cooks");

            migrationBuilder.DropForeignKey(
                name: "FK_Manager_Restaurants_RestaurantId",
                table: "Manager");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Cooks_CookId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Courier_CourierId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Courier");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CourierId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cooks",
                table: "Cooks");

            migrationBuilder.DropColumn(
                name: "CourierId",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "Cooks",
                newName: "Cook");

            migrationBuilder.RenameIndex(
                name: "IX_Cooks_RestaurantId",
                table: "Cook",
                newName: "IX_Cook_RestaurantId");

            migrationBuilder.AlterColumn<Guid>(
                name: "RestaurantId",
                table: "Manager",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "RestaurantId",
                table: "Cook",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cook",
                table: "Cook",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cook_Restaurants_RestaurantId",
                table: "Cook",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Manager_Restaurants_RestaurantId",
                table: "Manager",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Cook_CookId",
                table: "Orders",
                column: "CookId",
                principalTable: "Cook",
                principalColumn: "Id");
        }
    }
}
