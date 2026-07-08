import { Supplier, SuppliersQueryParams } from '../../features/suppliers/suppliers.models';

export interface SuppliersState {
  items: Supplier[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialSuppliersState: SuppliersState = {
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

export type SuppliersLoadParams = SuppliersQueryParams;
