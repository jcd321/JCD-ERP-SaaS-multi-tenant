import { InventoryMovement, MovementLookupOption, MovementsQueryParams } from '../../features/movements/movements.models';

export interface MovementsState {
  items: InventoryMovement[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  warehouseId: string | null;
  productId: string | null;
  movementType: string | null;
  productOptions: MovementLookupOption[];
  warehouseOptions: MovementLookupOption[];
  lookupsLoading: boolean;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialMovementsState: MovementsState = {
  items: [],
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,
  search: '',
  warehouseId: null,
  productId: null,
  movementType: null,
  productOptions: [],
  warehouseOptions: [],
  lookupsLoading: false,
  loading: false,
  saving: false,
  error: null,
};
