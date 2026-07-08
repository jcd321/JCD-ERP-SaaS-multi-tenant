import { Warehouse, WarehousesQueryParams } from '../../features/warehouses/warehouses.models';

export interface WarehousesState {
  items: Warehouse[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialWarehousesState: WarehousesState = {
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

export type WarehousesLoadParams = WarehousesQueryParams;
