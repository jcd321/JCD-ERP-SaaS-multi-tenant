export interface InventoryTransferLine {
  id: string;
  productId: string;
  productSku: string;
  productName: string;
  unitSymbol: string | null;
  quantity: number;
  lineNumber: number;
}

export interface InventoryTransfer {
  id: string;
  documentNumber: string;
  sourceWarehouseId: string;
  sourceWarehouseCode: string;
  sourceWarehouseName: string;
  destinationWarehouseId: string;
  destinationWarehouseCode: string;
  destinationWarehouseName: string;
  transferDate: string;
  notes: string | null;
  lineCount: number;
  lines: InventoryTransferLine[];
  createdAt: string;
}

export interface TransferLookupOption {
  id: string;
  label: string;
}

export interface TransferStockLevel {
  productId: string;
  warehouseId: string;
  quantityOnHand: number;
}

export interface TransferLookupsResponse {
  products: TransferLookupOption[];
  warehouses: TransferLookupOption[];
  stockLevels: TransferStockLevel[];
}

export interface PaginatedTransfersResponse {
  items: InventoryTransfer[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateTransferLineRequest {
  productId: string;
  quantity: number;
}

export interface CreateInventoryTransferRequest {
  sourceWarehouseId: string;
  destinationWarehouseId: string;
  transferDate?: string | null;
  notes?: string | null;
  lines: CreateTransferLineRequest[];
}

export type TransferFormMode = 'create' | null;

export interface TransfersQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  sourceWarehouseId?: string | null;
  destinationWarehouseId?: string | null;
  fromDate?: string | null;
  toDate?: string | null;
}
