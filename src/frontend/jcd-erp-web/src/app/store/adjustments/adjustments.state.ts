import {
  AdjustmentLookupOption,
  AdjustmentStockLevel,
  InventoryAdjustment,
} from '../../features/adjustments/adjustments.models';

export interface AdjustmentsState {
  items: InventoryAdjustment[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  warehouseId: string | null;
  productOptions: AdjustmentLookupOption[];
  warehouseOptions: AdjustmentLookupOption[];
  stockLevels: AdjustmentStockLevel[];
  lookupsLoading: boolean;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialAdjustmentsState: AdjustmentsState = {
  items: [],
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,
  search: '',
  warehouseId: null,
  productOptions: [],
  warehouseOptions: [],
  stockLevels: [],
  lookupsLoading: false,
  loading: false,
  saving: false,
  error: null,
};
