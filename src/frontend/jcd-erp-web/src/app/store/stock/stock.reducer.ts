import { createReducer, on } from '@ngrx/store';

import { StockActions } from './stock.actions';
import { initialStockState } from './stock.state';

export const stockReducer = createReducer(
  initialStockState,

  on(StockActions.loadStockLevels, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    warehouseId: params?.warehouseId !== undefined ? params.warehouseId ?? null : state.warehouseId,
    productId: params?.productId !== undefined ? params.productId ?? null : state.productId,
    belowMinimumOnly: params?.belowMinimumOnly ?? state.belowMinimumOnly,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(StockActions.loadStockLevelsSuccess, (state, { response, filters }) => ({
    ...state,
    items: response.items,
    page: response.page,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    totalPages: response.totalPages,
    search: filters.search ?? '',
    warehouseId: filters.warehouseId ?? null,
    productId: filters.productId ?? null,
    belowMinimumOnly: filters.belowMinimumOnly ?? false,
    loading: false,
    error: null,
  })),

  on(StockActions.loadStockLevelsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(StockActions.loadStockLookups, (state) => ({
    ...state,
    lookupsLoading: true,
  })),

  on(StockActions.loadStockLookupsSuccess, (state, { lookups }) => ({
    ...state,
    productOptions: lookups.products,
    warehouseOptions: lookups.warehouses,
    lookupsLoading: false,
  })),

  on(StockActions.loadStockLookupsFailure, (state) => ({
    ...state,
    lookupsLoading: false,
  })),

  on(StockActions.createStockLevel, StockActions.updateStockLevel, StockActions.deleteStockLevel, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(
    StockActions.createStockLevelSuccess,
    StockActions.updateStockLevelSuccess,
    StockActions.deleteStockLevelSuccess,
    (state) => ({
      ...state,
      saving: false,
      error: null,
    }),
  ),

  on(
    StockActions.createStockLevelFailure,
    StockActions.updateStockLevelFailure,
    StockActions.deleteStockLevelFailure,
    (state, { error }) => ({
      ...state,
      saving: false,
      error,
    }),
  ),
);
