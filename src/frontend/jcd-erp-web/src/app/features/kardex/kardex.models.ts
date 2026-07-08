export type MovementType = 'IN' | 'OUT';

export interface KardexEntry {
  id: string;
  documentNumber: string;
  productId: string;
  productSku: string;
  productName: string;
  unitSymbol: string | null;
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
  movementType: MovementType;
  quantity: number;
  quantityBefore: number;
  quantityAfter: number;
  reference: string | null;
  notes: string | null;
  movementDate: string;
  createdAt: string;
}

export interface KardexLookupOption {
  id: string;
  label: string;
}

export interface KardexLookupsResponse {
  products: KardexLookupOption[];
  warehouses: KardexLookupOption[];
}

export interface PaginatedKardexResponse {
  items: KardexEntry[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface KardexQueryParams {
  page?: number;
  pageSize?: number;
  productId?: string | null;
  warehouseId?: string | null;
  fromDate?: string | null;
  toDate?: string | null;
}
