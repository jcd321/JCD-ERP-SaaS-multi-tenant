export interface UnitOfMeasure {
  id: string;
  code: string;
  name: string;
  symbol: string | null;
  isActive: boolean;
}

export interface PaginatedUnitsResponse {
  items: UnitOfMeasure[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateUnitRequest {
  code: string;
  name: string;
  symbol?: string | null;
}

export interface UpdateUnitRequest {
  code: string;
  name: string;
  symbol?: string | null;
  isActive: boolean;
}

export type UnitFormMode = 'create' | 'edit' | null;

export interface UnitsQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean | null;
}
