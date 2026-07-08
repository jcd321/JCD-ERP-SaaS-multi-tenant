import { createFeatureSelector, createSelector } from '@ngrx/store';

import { WarehousesState } from './warehouses.state';

export const selectWarehousesState = createFeatureSelector<WarehousesState>('warehouses');

export const selectAllWarehouses = createSelector(selectWarehousesState, (state) => state.items);

export const selectWarehousesPage = createSelector(selectWarehousesState, (state) => state.page);

export const selectWarehousesPageSize = createSelector(selectWarehousesState, (state) => state.pageSize);

export const selectWarehousesTotalCount = createSelector(selectWarehousesState, (state) => state.totalCount);

export const selectWarehousesTotalPages = createSelector(selectWarehousesState, (state) => state.totalPages);

export const selectWarehousesSearch = createSelector(selectWarehousesState, (state) => state.search);

export const selectWarehousesLoading = createSelector(selectWarehousesState, (state) => state.loading);

export const selectWarehousesSaving = createSelector(selectWarehousesState, (state) => state.saving);

export const selectWarehousesError = createSelector(selectWarehousesState, (state) => state.error);
