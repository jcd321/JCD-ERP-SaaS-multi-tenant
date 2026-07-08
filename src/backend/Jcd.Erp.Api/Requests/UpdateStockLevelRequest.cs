namespace Jcd.Erp.Api.Requests;

public sealed class UpdateStockLevelRequest
{
    public decimal QuantityOnHand { get; init; }
    public decimal? MinQuantity { get; init; }
    public decimal? MaxQuantity { get; init; }
}
