export interface StockLevel {
  id: string;
  productId: string;
  productSku: string;
  productName: string;
  unitSymbol: string | null;
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
  quantityOnHand: number;
  minQuantity: number | null;
  maxQuantity: number | null;
  isBelowMinimum: boolean;
}

export interface StockLookupOption {
  id: string;
  label: string;
}

export interface StockLookupsResponse {
  products: StockLookupOption[];
  warehouses: StockLookupOption[];
}

export interface PaginatedStockResponse {
  items: StockLevel[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateStockLevelRequest {
  productId: string;
  warehouseId: string;
  quantityOnHand: number;
  minQuantity?: number | null;
  maxQuantity?: number | null;
}

export interface UpdateStockLevelRequest {
  quantityOnHand: number;
  minQuantity?: number | null;
  maxQuantity?: number | null;
}

export type StockFormMode = 'create' | 'edit' | null;

export interface StockQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  warehouseId?: string | null;
  productId?: string | null;
  belowMinimumOnly?: boolean | null;
}
