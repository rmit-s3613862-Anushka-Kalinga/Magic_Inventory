using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Magic_Inventory.Data.Migrations
{
    public partial class OrderHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrder_Product_ProductID",
                table: "CustomerOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrder_Store_StoreID",
                table: "CustomerOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerOrder",
                table: "CustomerOrder");

            migrationBuilder.RenameTable(
                name: "CustomerOrder",
                newName: "OrderHistory");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerOrder_StoreID",
                table: "OrderHistory",
                newName: "IX_OrderHistory_StoreID");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerOrder_ProductID",
                table: "OrderHistory",
                newName: "IX_OrderHistory_ProductID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderHistory",
                table: "OrderHistory",
                column: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHistory_Product_ProductID",
                table: "OrderHistory",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHistory_Store_StoreID",
                table: "OrderHistory",
                column: "StoreID",
                principalTable: "Store",
                principalColumn: "StoreID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHistory_Product_ProductID",
                table: "OrderHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHistory_Store_StoreID",
                table: "OrderHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderHistory",
                table: "OrderHistory");

            migrationBuilder.RenameTable(
                name: "OrderHistory",
                newName: "CustomerOrder");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHistory_StoreID",
                table: "CustomerOrder",
                newName: "IX_CustomerOrder_StoreID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHistory_ProductID",
                table: "CustomerOrder",
                newName: "IX_CustomerOrder_ProductID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerOrder",
                table: "CustomerOrder",
                column: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOrder_Product_ProductID",
                table: "CustomerOrder",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOrder_Store_StoreID",
                table: "CustomerOrder",
                column: "StoreID",
                principalTable: "Store",
                principalColumn: "StoreID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
