import { Brand, BrandsQueryParams } from '../../features/brands/brands.models';

export interface BrandsState {
  items: Brand[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialBrandsState: BrandsState = {
  items: [],
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,
  search: '',
  loading: false,
  saving: false,
  error: null,
};

export type BrandsLoadParams = BrandsQueryParams;
