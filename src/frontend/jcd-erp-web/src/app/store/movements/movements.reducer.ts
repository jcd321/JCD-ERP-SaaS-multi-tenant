import { createReducer, on } from '@ngrx/store';

import { MovementsActions } from './movements.actions';
import { initialMovementsState } from './movements.state';

export const movementsReducer = createReducer(
  initialMovementsState,

  on(MovementsActions.loadMovements, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    warehouseId: params?.warehouseId !== undefined ? params.warehouseId ?? null : state.warehouseId,
    productId: params?.productId !== undefined ? params.productId ?? null : state.productId,
    movementType: params?.movementType !== undefined ? params.movementType ?? null : state.movementType,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(MovementsActions.loadMovementsSuccess, (state, { response, filters }) => ({
    ...state,
    items: response.items,
    page: response.page,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    totalPages: response.totalPages,
    search: filters.search ?? '',
    warehouseId: filters.warehouseId ?? null,
    productId: filters.productId ?? null,
    movementType: filters.movementType ?? null,
    loading: false,
    error: null,
  })),

  on(MovementsActions.loadMovementsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(MovementsActions.loadMovementLookups, (state) => ({
    ...state,
    lookupsLoading: true,
  })),

  on(MovementsActions.loadMovementLookupsSuccess, (state, { lookups }) => ({
    ...state,
    productOptions: lookups.products,
    warehouseOptions: lookups.warehouses,
    lookupsLoading: false,
  })),

  on(MovementsActions.loadMovementLookupsFailure, (state) => ({
    ...state,
    lookupsLoading: false,
  })),

  on(MovementsActions.createMovement, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(MovementsActions.createMovementSuccess, (state) => ({
    ...state,
    saving: false,
    error: null,
  })),

  on(MovementsActions.createMovementFailure, (state, { error }) => ({
    ...state,
    saving: false,
    error,
  })),
);
