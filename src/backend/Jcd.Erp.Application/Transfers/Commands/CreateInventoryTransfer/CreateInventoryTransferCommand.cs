using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Transfers.Commands.CreateInventoryTransfer;

public record CreateInventoryTransferLineRequest(Guid ProductId, decimal Quantity);

public record CreateInventoryTransferCommand(
    Guid SourceWarehouseId,
    Guid DestinationWarehouseId,
    DateTime? TransferDate,
    string? Notes,
    IReadOnlyList<CreateInventoryTransferLineRequest> Lines) : IRequest<Result<Guid>>;
