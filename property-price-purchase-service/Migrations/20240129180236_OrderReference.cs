﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace property_price_purchase_service.Migrations
{
    /// <inheritdoc />
    public partial class OrderReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                table: "Orders");
        }
    }
}
