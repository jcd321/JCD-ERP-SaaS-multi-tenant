export interface Product {
  id: string;
  sku: string;
  name: string;
  description: string | null;
  categoryId: string;
  categoryName: string;
  brandId: string | null;
  brandName: string | null;
  unitId: string;
  unitName: string;
  isActive: boolean;
}

export interface LookupOption {
  id: string;
  name: string;
}

export interface ProductLookups {
  categories: LookupOption[];
  brands: LookupOption[];
  units: LookupOption[];
}

export interface PaginatedProductsResponse {
  items: Product[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateProductRequest {
  sku: string;
  name: string;
  categoryId: string;
  unitId: string;
  description?: string | null;
  brandId?: string | null;
}

export interface UpdateProductRequest {
  sku: string;
  name: string;
  categoryId: string;
  unitId: string;
  description?: string | null;
  brandId?: string | null;
  isActive: boolean;
}

export type ProductFormMode = 'create' | 'edit' | null;

export interface ProductsQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean | null;
}
