import { createReducer, on } from '@ngrx/store';

import { PhysicalCountsActions } from './physical-counts.actions';
import { initialPhysicalCountsState } from './physical-counts.state';

export const physicalCountsReducer = createReducer(
  initialPhysicalCountsState,

  on(PhysicalCountsActions.loadPhysicalCounts, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    warehouseId: params?.warehouseId !== undefined ? params.warehouseId ?? null : state.warehouseId,
    status: params?.status !== undefined ? params.status ?? null : state.status,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(PhysicalCountsActions.loadPhysicalCountsSuccess, (state, { response, filters }) => ({
    ...state,
    items: response.items,
    page: response.page,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    totalPages: response.totalPages,
    search: filters.search ?? '',
    warehouseId: filters.warehouseId ?? null,
    status: filters.status ?? null,
    loading: false,
    error: null,
  })),

  on(PhysicalCountsActions.loadPhysicalCountsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(PhysicalCountsActions.loadPhysicalCount, (state) => ({
    ...state,
    detailLoading: true,
    error: null,
  })),

  on(PhysicalCountsActions.loadPhysicalCountSuccess, (state, { count }) => ({
    ...state,
    selectedCount: count,
    detailLoading: false,
    error: null,
  })),

  on(PhysicalCountsActions.loadPhysicalCountFailure, (state, { error }) => ({
    ...state,
    detailLoading: false,
    error,
  })),

  on(PhysicalCountsActions.loadPhysicalCountLookups, (state) => ({
    ...state,
    lookupsLoading: true,
  })),

  on(PhysicalCountsActions.loadPhysicalCountLookupsSuccess, (state, { lookups }) => ({
    ...state,
    warehouseOptions: lookups.warehouses,
    lookupsLoading: false,
  })),

  on(PhysicalCountsActions.loadPhysicalCountLookupsFailure, (state) => ({
    ...state,
    lookupsLoading: false,
  })),

  on(
    PhysicalCountsActions.createPhysicalCount,
    PhysicalCountsActions.updatePhysicalCountLines,
    PhysicalCountsActions.completePhysicalCount,
    PhysicalCountsActions.cancelPhysicalCount,
    (state) => ({
      ...state,
      saving: true,
      error: null,
    }),
  ),

  on(
    PhysicalCountsActions.createPhysicalCountSuccess,
    PhysicalCountsActions.updatePhysicalCountLinesSuccess,
    PhysicalCountsActions.completePhysicalCountSuccess,
    PhysicalCountsActions.cancelPhysicalCountSuccess,
    (state) => ({
      ...state,
      saving: false,
      error: null,
    }),
  ),

  on(
    PhysicalCountsActions.createPhysicalCountFailure,
    PhysicalCountsActions.updatePhysicalCountLinesFailure,
    PhysicalCountsActions.completePhysicalCountFailure,
    PhysicalCountsActions.cancelPhysicalCountFailure,
    (state, { error }) => ({
      ...state,
      saving: false,
      error,
    }),
  ),

  on(PhysicalCountsActions.clearError, (state) => ({
    ...state,
    error: null,
  })),
);
