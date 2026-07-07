import { Product, ProductLookups, ProductsQueryParams } from '../../features/products/products.models';

export interface ProductsState {
  items: Product[];
  lookups: ProductLookups;
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialProductsState: ProductsState = {
  items: [],
  lookups: { categories: [], brands: [], units: [] },
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,
  search: '',
  loading: false,
  saving: false,
  error: null,
};

export type ProductsLoadParams = ProductsQueryParams;
