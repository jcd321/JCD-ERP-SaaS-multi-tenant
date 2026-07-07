import { CategoryParentOption, ProductCategory, CategoriesQueryParams } from '../../features/categories/categories.models';

export interface CategoriesState {
  items: ProductCategory[];
  parentOptions: CategoryParentOption[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialCategoriesState: CategoriesState = {
  items: [],
  parentOptions: [],
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,
  search: '',
  loading: false,
  saving: false,
  error: null,
};

export type CategoriesLoadParams = CategoriesQueryParams;
