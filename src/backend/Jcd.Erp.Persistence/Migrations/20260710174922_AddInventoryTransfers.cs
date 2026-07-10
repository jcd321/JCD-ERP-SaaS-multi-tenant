using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jcd.Erp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryTransfers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inventory_transfers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    source_warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    destination_warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transfer_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_inventory_transfers", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_transfers_warehouses_destination_warehouse_id",
                        column: x => x.destination_warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventory_transfers_warehouses_source_warehouse_id",
                        column: x => x.source_warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inventory_transfer_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transfer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_transfer_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_transfer_lines_inventory_transfers_transfer_id",
                        column: x => x.transfer_id,
                        principalTable: "inventory_transfers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inventory_transfer_lines_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transfer_lines_product_id",
                table: "inventory_transfer_lines",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transfer_lines_transfer_id_line_number",
                table: "inventory_transfer_lines",
                columns: new[] { "transfer_id", "line_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transfers_destination_warehouse_id",
                table: "inventory_transfers",
                column: "destination_warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transfers_source_warehouse_id",
                table: "inventory_transfers",
                column: "source_warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transfers_tenant_id_document_number",
                table: "inventory_transfers",
                columns: new[] { "tenant_id", "document_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transfers_tenant_id_transfer_date",
                table: "inventory_transfers",
                columns: new[] { "tenant_id", "transfer_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_transfer_lines");

            migrationBuilder.DropTable(
                name: "inventory_transfers");
        }
    }
}
