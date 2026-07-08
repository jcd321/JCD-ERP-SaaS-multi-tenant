export type MovementType = 'IN' | 'OUT';

export interface InventoryMovement {
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

export interface MovementLookupOption {
  id: string;
  label: string;
}

export interface MovementLookupsResponse {
  products: MovementLookupOption[];
  warehouses: MovementLookupOption[];
}

export interface PaginatedMovementsResponse {
  items: InventoryMovement[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateInventoryMovementRequest {
  productId: string;
  warehouseId: string;
  movementType: MovementType;
  quantity: number;
  movementDate?: string | null;
  reference?: string | null;
  notes?: string | null;
}

export type MovementFormMode = 'create' | null;

export interface MovementsQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  warehouseId?: string | null;
  productId?: string | null;
  movementType?: MovementType | null;
}

export const MOVEMENT_TYPES: MovementType[] = ['IN', 'OUT'];
