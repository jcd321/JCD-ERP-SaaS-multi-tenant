using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jcd.Erp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPhysicalInventoryCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "physical_inventory_counts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    count_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_physical_inventory_counts", x => x.id);
                    table.ForeignKey(
                        name: "FK_physical_inventory_counts_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "physical_inventory_count_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    count_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    system_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    counted_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    line_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_physical_inventory_count_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_physical_inventory_count_lines_physical_inventory_counts_co~",
                        column: x => x.count_id,
                        principalTable: "physical_inventory_counts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_physical_inventory_count_lines_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_physical_inventory_count_lines_count_id_line_number",
                table: "physical_inventory_count_lines",
                columns: new[] { "count_id", "line_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_physical_inventory_count_lines_product_id",
                table: "physical_inventory_count_lines",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_physical_inventory_counts_tenant_id_count_date",
                table: "physical_inventory_counts",
                columns: new[] { "tenant_id", "count_date" });

            migrationBuilder.CreateIndex(
                name: "ix_physical_inventory_counts_tenant_id_document_number",
                table: "physical_inventory_counts",
                columns: new[] { "tenant_id", "document_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_physical_inventory_counts_warehouse_id",
                table: "physical_inventory_counts",
                column: "warehouse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "physical_inventory_count_lines");

            migrationBuilder.DropTable(
                name: "physical_inventory_counts");
        }
    }
}
