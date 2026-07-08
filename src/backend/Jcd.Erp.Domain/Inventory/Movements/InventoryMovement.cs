using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Warehouses;

namespace Jcd.Erp.Domain.Inventory.Movements;

public class InventoryMovement : BaseAuditableEntity
{
    public string DocumentNumber { get; private set; } = string.Empty;
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string MovementType { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal QuantityBefore { get; private set; }
    public decimal QuantityAfter { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public DateTime MovementDate { get; private set; }

    public Product Product { get; private set; } = null!;
    public Warehouse Warehouse { get; private set; } = null!;

    private InventoryMovement() { }

    public static Result<InventoryMovement> Create(
        Guid tenantId,
        string documentNumber,
        Guid productId,
        Guid warehouseId,
        string movementType,
        decimal quantity,
        decimal quantityBefore,
        decimal quantityAfter,
        DateTime movementDate,
        string? reference = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty)
            return Result.Failure<InventoryMovement>("Movement.TenantRequired");

        if (string.IsNullOrWhiteSpace(documentNumber))
            return Result.Failure<InventoryMovement>("Movement.DocumentNumberRequired");

        if (productId == Guid.Empty)
            return Result.Failure<InventoryMovement>("Movement.ProductRequired");

        if (warehouseId == Guid.Empty)
            return Result.Failure<InventoryMovement>("Movement.WarehouseRequired");

        if (!InventoryMovementTypes.IsValid(movementType))
            return Result.Failure<InventoryMovement>("Movement.TypeInvalid");

        if (quantity <= 0)
            return Result.Failure<InventoryMovement>("Movement.QuantityInvalid");

        if (quantityBefore < 0 || quantityAfter < 0)
            return Result.Failure<InventoryMovement>("Movement.QuantityInvalid");

        return Result.Success(new InventoryMovement
        {
            TenantId = tenantId,
            DocumentNumber = documentNumber.Trim().ToUpperInvariant(),
            ProductId = productId,
            WarehouseId = warehouseId,
            MovementType = InventoryMovementTypes.Normalize(movementType),
            Quantity = quantity,
            QuantityBefore = quantityBefore,
            QuantityAfter = quantityAfter,
            Reference = NormalizeOptional(reference),
            Notes = NormalizeOptional(notes),
            MovementDate = movementDate,
            CreatedAt = DateTime.UtcNow,
        });
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
