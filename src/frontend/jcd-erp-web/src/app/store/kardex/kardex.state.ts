import { KardexEntry, KardexLookupOption } from '../../features/kardex/kardex.models';

export interface KardexState {
  items: KardexEntry[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  productId: string | null;
  warehouseId: string | null;
  fromDate: string | null;
  toDate: string | null;
  productOptions: KardexLookupOption[];
  warehouseOptions: KardexLookupOption[];
  lookupsLoading: boolean;
  loading: boolean;
  error: string | null;
}

export const initialKardexState: KardexState = {
  items: [],
  page: 1,
  pageSize: 50,
  totalCount: 0,
  totalPages: 0,
  productId: null,
  warehouseId: null,
  fromDate: null,
  toDate: null,
  productOptions: [],
  warehouseOptions: [],
  lookupsLoading: false,
  loading: false,
  error: null,
};
