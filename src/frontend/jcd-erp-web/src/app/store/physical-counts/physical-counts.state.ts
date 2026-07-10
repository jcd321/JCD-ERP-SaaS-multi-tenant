import {
  PhysicalCountLookupOption,
  PhysicalInventoryCount,
} from '../../features/physical-counts/physical-counts.models';

export interface PhysicalCountsState {
  items: PhysicalInventoryCount[];
  selectedCount: PhysicalInventoryCount | null;
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  warehouseId: string | null;
  status: string | null;
  loading: boolean;
  detailLoading: boolean;
  saving: boolean;
  error: string | null;
  warehouseOptions: PhysicalCountLookupOption[];
  lookupsLoading: boolean;
}

export const initialPhysicalCountsState: PhysicalCountsState = {
  items: [],
  selectedCount: null,
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,
  search: '',
  warehouseId: null,
  status: null,
  loading: false,
  detailLoading: false,
  saving: false,
  error: null,
  warehouseOptions: [],
  lookupsLoading: false,
};
