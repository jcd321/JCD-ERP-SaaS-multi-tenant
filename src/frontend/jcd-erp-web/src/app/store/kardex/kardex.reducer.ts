import { createReducer, on } from '@ngrx/store';

import { KardexActions } from './kardex.actions';
import { initialKardexState } from './kardex.state';

export const kardexReducer = createReducer(
  initialKardexState,

  on(KardexActions.loadKardex, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    productId: params?.productId !== undefined ? params.productId ?? null : state.productId,
    warehouseId: params?.warehouseId !== undefined ? params.warehouseId ?? null : state.warehouseId,
    fromDate: params?.fromDate !== undefined ? params.fromDate ?? null : state.fromDate,
    toDate: params?.toDate !== undefined ? params.toDate ?? null : state.toDate,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(KardexActions.loadKardexSuccess, (state, { response, filters }) => ({
    ...state,
    items: response.items,
    page: response.page,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    totalPages: response.totalPages,
    productId: filters.productId ?? null,
    warehouseId: filters.warehouseId ?? null,
    fromDate: filters.fromDate ?? null,
    toDate: filters.toDate ?? null,
    loading: false,
    error: null,
  })),

  on(KardexActions.loadKardexFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(KardexActions.loadKardexLookups, (state) => ({
    ...state,
    lookupsLoading: true,
  })),

  on(KardexActions.loadKardexLookupsSuccess, (state, { lookups }) => ({
    ...state,
    productOptions: lookups.products,
    warehouseOptions: lookups.warehouses,
    lookupsLoading: false,
  })),

  on(KardexActions.loadKardexLookupsFailure, (state) => ({
    ...state,
    lookupsLoading: false,
  })),
);
