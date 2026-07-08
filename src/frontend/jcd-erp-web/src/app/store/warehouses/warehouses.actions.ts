import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  WarehousesQueryParams,
  CreateWarehouseRequest,
  PaginatedWarehousesResponse,
  UpdateWarehouseRequest,
} from '../../features/warehouses/warehouses.models';

export const WarehousesActions = createActionGroup({
  source: 'Warehouses',
  events: {
    'Load Warehouses': props<{ params?: WarehousesQueryParams }>(),
    'Load Warehouses Success': props<{ response: PaginatedWarehousesResponse; search: string }>(),
    'Load Warehouses Failure': props<{ error: string }>(),

    'Create Warehouse': props<{ request: CreateWarehouseRequest }>(),
    'Create Warehouse Success': emptyProps(),
    'Create Warehouse Failure': props<{ error: string }>(),

    'Update Warehouse': props<{ warehouseId: string; request: UpdateWarehouseRequest }>(),
    'Update Warehouse Success': emptyProps(),
    'Update Warehouse Failure': props<{ error: string }>(),

    'Delete Warehouse': props<{ warehouseId: string }>(),
    'Delete Warehouse Success': emptyProps(),
    'Delete Warehouse Failure': props<{ error: string }>(),
  },
});
