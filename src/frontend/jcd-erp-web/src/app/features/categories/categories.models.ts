export interface ProductCategory {
  id: string;
  name: string;
  description: string | null;
  parentId: string | null;
  parentName: string | null;
  isActive: boolean;
}

export interface CategoryParentOption {
  id: string;
  name: string;
}

export interface PaginatedCategoriesResponse {
  items: ProductCategory[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string | null;
  parentId?: string | null;
}

export interface UpdateCategoryRequest {
  name: string;
  description?: string | null;
  parentId?: string | null;
  isActive: boolean;
}

export type CategoryFormMode = 'create' | 'edit' | null;

export interface CategoriesQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean | null;
}
