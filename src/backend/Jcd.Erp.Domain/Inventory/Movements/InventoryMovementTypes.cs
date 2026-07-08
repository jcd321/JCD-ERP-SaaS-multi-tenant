namespace Jcd.Erp.Domain.Inventory.Movements;

public static class InventoryMovementTypes
{
    public const string In = "IN";
    public const string Out = "OUT";

    public static bool IsValid(string? value) =>
        string.Equals(value, In, StringComparison.OrdinalIgnoreCase)
        || string.Equals(value, Out, StringComparison.OrdinalIgnoreCase);

    public static string Normalize(string value) =>
        string.Equals(value, Out, StringComparison.OrdinalIgnoreCase) ? Out : In;
}
