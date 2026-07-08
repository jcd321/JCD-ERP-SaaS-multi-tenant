export interface StorageLocation {
  id: string;
  warehouseId: string;
  code: string;
  name: string;
  description: string | null;
  parentId: string | null;
  parentName: string | null;
  locationType: string | null;
  isActive: boolean;
}

export interface LocationParentOption {
  id: string;
  name: string;
  code: string;
}

export interface PaginatedLocationsResponse {
  items: StorageLocation[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateLocationRequest {
  warehouseId: string;
  code: string;
  name: string;
  description?: string | null;
  parentId?: string | null;
  locationType?: string | null;
}

export interface UpdateLocationRequest {
  code: string;
  name: string;
  description?: string | null;
  parentId?: string | null;
  locationType?: string | null;
  isActive: boolean;
}

export type LocationFormMode = 'create' | 'edit' | null;

export interface LocationsQueryParams {
  warehouseId: string;
  page?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean | null;
}
