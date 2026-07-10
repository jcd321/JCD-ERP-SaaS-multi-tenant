import { createReducer, on } from '@ngrx/store';

import { AdjustmentsActions } from './adjustments.actions';
import { initialAdjustmentsState } from './adjustments.state';

export const adjustmentsReducer = createReducer(
  initialAdjustmentsState,

  on(AdjustmentsActions.loadAdjustments, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    warehouseId: params?.warehouseId !== undefined ? params.warehouseId ?? null : state.warehouseId,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(AdjustmentsActions.loadAdjustmentsSuccess, (state, { response, filters }) => ({
    ...state,
    items: response.items,
    page: response.page,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    totalPages: response.totalPages,
    search: filters.search ?? '',
    warehouseId: filters.warehouseId ?? null,
    loading: false,
    error: null,
  })),

  on(AdjustmentsActions.loadAdjustmentsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(AdjustmentsActions.loadAdjustmentLookups, (state) => ({
    ...state,
    lookupsLoading: true,
  })),

  on(AdjustmentsActions.loadAdjustmentLookupsSuccess, (state, { lookups }) => ({
    ...state,
    productOptions: lookups.products,
    warehouseOptions: lookups.warehouses,
    stockLevels: lookups.stockLevels ?? [],
    lookupsLoading: false,
  })),

  on(AdjustmentsActions.loadAdjustmentLookupsFailure, (state) => ({
    ...state,
    lookupsLoading: false,
  })),

  on(AdjustmentsActions.createAdjustment, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(AdjustmentsActions.createAdjustmentSuccess, (state) => ({
    ...state,
    saving: false,
    error: null,
  })),

  on(AdjustmentsActions.createAdjustmentFailure, (state, { error }) => ({
    ...state,
    saving: false,
    error,
  })),

  on(AdjustmentsActions.clearError, (state) => ({
    ...state,
    error: null,
  })),
);
