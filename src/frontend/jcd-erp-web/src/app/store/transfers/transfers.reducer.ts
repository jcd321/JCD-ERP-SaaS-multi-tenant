import { createReducer, on } from '@ngrx/store';

import { TransfersActions } from './transfers.actions';
import { initialTransfersState } from './transfers.state';

export const transfersReducer = createReducer(
  initialTransfersState,

  on(TransfersActions.loadTransfers, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    sourceWarehouseId: params?.sourceWarehouseId !== undefined ? params.sourceWarehouseId ?? null : state.sourceWarehouseId,
    destinationWarehouseId: params?.destinationWarehouseId !== undefined ? params.destinationWarehouseId ?? null : state.destinationWarehouseId,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(TransfersActions.loadTransfersSuccess, (state, { response, filters }) => ({
    ...state,
    items: response.items,
    page: response.page,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    totalPages: response.totalPages,
    search: filters.search ?? '',
    sourceWarehouseId: filters.sourceWarehouseId ?? null,
    destinationWarehouseId: filters.destinationWarehouseId ?? null,
    loading: false,
    error: null,
  })),

  on(TransfersActions.loadTransfersFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(TransfersActions.loadTransferLookups, (state) => ({
    ...state,
    lookupsLoading: true,
  })),

  on(TransfersActions.loadTransferLookupsSuccess, (state, { lookups }) => ({
    ...state,
    productOptions: lookups.products,
    warehouseOptions: lookups.warehouses,
    stockLevels: lookups.stockLevels ?? [],
    lookupsLoading: false,
  })),

  on(TransfersActions.loadTransferLookupsFailure, (state) => ({
    ...state,
    lookupsLoading: false,
  })),

  on(TransfersActions.createTransfer, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(TransfersActions.createTransferSuccess, (state) => ({
    ...state,
    saving: false,
    error: null,
  })),

  on(TransfersActions.createTransferFailure, (state, { error }) => ({
    ...state,
    saving: false,
    error,
  })),

  on(TransfersActions.clearError, (state) => ({
    ...state,
    error: null,
  })),
);
