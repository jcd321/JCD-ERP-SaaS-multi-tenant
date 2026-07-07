export interface Brand {
  id: string;
  code: string;
  name: string;
  description: string | null;
  isActive: boolean;
}

export interface PaginatedBrandsResponse {
  items: Brand[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateBrandRequest {
  code: string;
  name: string;
  description?: string | null;
}

export interface UpdateBrandRequest {
  code: string;
  name: string;
  description?: string | null;
  isActive: boolean;
}

export type BrandFormMode = 'create' | 'edit' | null;

export interface BrandsQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean | null;
}
