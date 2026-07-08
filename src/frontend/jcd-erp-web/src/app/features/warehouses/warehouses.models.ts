export interface Warehouse {
  id: string;
  code: string;
  name: string;
  description: string | null;
  addressLine1: string | null;
  city: string | null;
  stateOrProvince: string | null;
  countryCode: string | null;
  isDefault: boolean;
  isActive: boolean;
}

export interface PaginatedWarehousesResponse {
  items: Warehouse[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateWarehouseRequest {
  code: string;
  name: string;
  description?: string | null;
  addressLine1?: string | null;
  city?: string | null;
  stateOrProvince?: string | null;
  countryCode?: string | null;
  isDefault?: boolean;
}

export interface UpdateWarehouseRequest {
  code: string;
  name: string;
  description?: string | null;
  addressLine1?: string | null;
  city?: string | null;
  stateOrProvince?: string | null;
  countryCode?: string | null;
  isDefault: boolean;
  isActive: boolean;
}

export type WarehouseFormMode = 'create' | 'edit' | null;

export interface WarehousesQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean | null;
}
