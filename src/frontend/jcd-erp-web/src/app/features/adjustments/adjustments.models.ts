export interface InventoryAdjustmentLine {
  id: string;
  productId: string;
  productSku: string;
  productName: string;
  unitSymbol: string | null;
  quantityBefore: number;
  quantityAfter: number;
  lineNumber: number;
}

export interface InventoryAdjustment {
  id: string;
  documentNumber: string;
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
  adjustmentDate: string;
  reason: string;
  notes: string | null;
  lineCount: number;
  lines: InventoryAdjustmentLine[];
  createdAt: string;
}

export interface AdjustmentLookupOption {
  id: string;
  label: string;
}

export interface AdjustmentStockLevel {
  productId: string;
  warehouseId: string;
  quantityOnHand: number;
}

export interface AdjustmentLookupsResponse {
  products: AdjustmentLookupOption[];
  warehouses: AdjustmentLookupOption[];
  stockLevels: AdjustmentStockLevel[];
}

export interface PaginatedAdjustmentsResponse {
  items: InventoryAdjustment[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateAdjustmentLineRequest {
  productId: string;
  countedQuantity: number;
}

export interface CreateInventoryAdjustmentRequest {
  warehouseId: string;
  adjustmentDate?: string | null;
  reason: string;
  notes?: string | null;
  lines: CreateAdjustmentLineRequest[];
}

export type AdjustmentFormMode = 'create' | null;

export interface AdjustmentsQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  warehouseId?: string | null;
  fromDate?: string | null;
  toDate?: string | null;
}
