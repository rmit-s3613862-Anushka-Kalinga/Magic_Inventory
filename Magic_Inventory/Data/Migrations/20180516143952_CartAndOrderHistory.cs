using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Magic_Inventory.Data.Migrations
{
    public partial class CartAndOrderHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrder_OrderDetail_OrderDetailID",
                table: "CustomerOrder");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOrder_OrderDetailID",
                table: "CustomerOrder");

            migrationBuilder.DropColumn(
                name: "OrderDetailID",
                table: "CustomerOrder");

            migrationBuilder.RenameColumn(
                name: "CustomerOrderID",
                table: "CustomerOrder",
                newName: "OrderID");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "CustomerOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "CustomerOrder",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "CustomerOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "CustomerOrder",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    CartID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CartEntryDate = table.Column<DateTime>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    ProductID = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    StoreID = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.CartID);
                    table.ForeignKey(
                        name: "FK_Cart_Product_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cart_Store_StoreID",
                        column: x => x.StoreID,
                        principalTable: "Store",
                        principalColumn: "StoreID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cart_ProductID",
                table: "Cart",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_StoreID",
                table: "Cart",
                column: "StoreID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "CustomerOrder");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "CustomerOrder");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "CustomerOrder");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "CustomerOrder");

            migrationBuilder.RenameColumn(
                name: "OrderID",
                table: "CustomerOrder",
                newName: "CustomerOrderID");

            migrationBuilder.AddColumn<int>(
                name: "OrderDetailID",
                table: "CustomerOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerEmail = table.Column<string>(nullable: true),
                    OrderDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.OrderDetailID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrder_OrderDetailID",
                table: "CustomerOrder",
                column: "OrderDetailID");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOrder_OrderDetail_OrderDetailID",
                table: "CustomerOrder",
                column: "OrderDetailID",
                principalTable: "OrderDetail",
                principalColumn: "OrderDetailID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
