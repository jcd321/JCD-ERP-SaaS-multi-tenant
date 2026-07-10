import { InventoryTransfer, TransferLookupOption, TransferStockLevel } from '../../features/transfers/transfers.models';

export interface TransfersState {
  items: InventoryTransfer[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  sourceWarehouseId: string | null;
  destinationWarehouseId: string | null;
  productOptions: TransferLookupOption[];
  warehouseOptions: TransferLookupOption[];
  stockLevels: TransferStockLevel[];
  lookupsLoading: boolean;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialTransfersState: TransfersState = {
  items: [],
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,
  search: '',
  sourceWarehouseId: null,
  destinationWarehouseId: null,
  productOptions: [],
  warehouseOptions: [],
  stockLevels: [],
  lookupsLoading: false,
  loading: false,
  saving: false,
  error: null,
};
