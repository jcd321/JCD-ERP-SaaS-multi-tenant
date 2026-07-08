using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jcd.Erp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stock_levels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity_on_hand = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    min_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    max_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_levels", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_levels_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_levels_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stock_levels_product_id",
                table: "stock_levels",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_stock_levels_tenant_id_is_deleted",
                table: "stock_levels",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_stock_levels_tenant_id_product_id_warehouse_id",
                table: "stock_levels",
                columns: new[] { "tenant_id", "product_id", "warehouse_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stock_levels_tenant_id_warehouse_id_is_deleted",
                table: "stock_levels",
                columns: new[] { "tenant_id", "warehouse_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "IX_stock_levels_warehouse_id",
                table: "stock_levels",
                column: "warehouse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_levels");
        }
    }
}
