import { createReducer, on } from '@ngrx/store';

import { WarehousesActions } from './warehouses.actions';
import { initialWarehousesState } from './warehouses.state';

export const warehousesReducer = createReducer(
  initialWarehousesState,

  on(WarehousesActions.loadWarehouses, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(WarehousesActions.loadWarehousesSuccess, (state, { response, search }) => ({
    ...state,
    items: response.items,
    page: response.page,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    totalPages: response.totalPages,
    search,
    loading: false,
    error: null,
  })),

  on(WarehousesActions.loadWarehousesFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(WarehousesActions.createWarehouse, WarehousesActions.updateWarehouse, WarehousesActions.deleteWarehouse, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(
    WarehousesActions.createWarehouseSuccess,
    WarehousesActions.updateWarehouseSuccess,
    WarehousesActions.deleteWarehouseSuccess,
    (state) => ({
      ...state,
      saving: false,
      error: null,
    }),
  ),

  on(
    WarehousesActions.createWarehouseFailure,
    WarehousesActions.updateWarehouseFailure,
    WarehousesActions.deleteWarehouseFailure,
    (state, { error }) => ({
      ...state,
      saving: false,
      error,
    }),
  ),
);
