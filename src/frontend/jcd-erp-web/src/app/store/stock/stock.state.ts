import { StockLevel, StockLookupOption, StockQueryParams } from '../../features/stock/stock.models';

export interface StockState {
  items: StockLevel[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  warehouseId: string | null;
  productId: string | null;
  belowMinimumOnly: boolean;
  productOptions: StockLookupOption[];
  warehouseOptions: StockLookupOption[];
  lookupsLoading: boolean;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialStockState: StockState = {
  items: [],
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,
  search: '',
  warehouseId: null,
  productId: null,
  belowMinimumOnly: false,
  productOptions: [],
  warehouseOptions: [],
  lookupsLoading: false,
  loading: false,
  saving: false,
  error: null,
};

export type StockLoadParams = StockQueryParams;
