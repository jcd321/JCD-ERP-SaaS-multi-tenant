export type PhysicalCountFormMode = 'create' | null;

export type PhysicalCountStatus = 'DRAFT' | 'COMPLETED' | 'CANCELLED';

export interface PhysicalCountLine {
  id: string;
  productId: string;
  productSku: string;
  productName: string;
  unitSymbol: string | null;
  systemQuantity: number;
  countedQuantity: number | null;
  lineNumber: number;
  hasVariance: boolean;
}

export interface PhysicalInventoryCount {
  id: string;
  documentNumber: string;
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
  countDate: string;
  status: PhysicalCountStatus;
  notes: string | null;
  lineCount: number;
  countedLineCount: number;
  varianceLineCount: number;
  lines: PhysicalCountLine[];
  createdAt: string;
  completedAt: string | null;
}

export interface PaginatedPhysicalCountsResponse {
  items: PhysicalInventoryCount[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface PhysicalCountsQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  warehouseId?: string;
  status?: string;
  fromDate?: string;
  toDate?: string;
}

export interface PhysicalCountLookupOption {
  id: string;
  label: string;
}

export interface PhysicalCountLookupsResponse {
  warehouses: PhysicalCountLookupOption[];
}

export interface CreatePhysicalCountRequest {
  warehouseId: string;
  countDate?: string;
  notes?: string | null;
}

export interface PhysicalCountLineUpdate {
  lineId: string;
  countedQuantity: number | null;
}
