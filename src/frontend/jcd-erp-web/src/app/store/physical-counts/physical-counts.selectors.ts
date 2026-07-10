import { createFeatureSelector, createSelector } from '@ngrx/store';

import { PhysicalCountsState } from './physical-counts.state';

export const selectPhysicalCountsState = createFeatureSelector<PhysicalCountsState>('physicalCounts');

export const selectPhysicalCounts = createSelector(selectPhysicalCountsState, (state) => state.items);
export const selectSelectedPhysicalCount = createSelector(selectPhysicalCountsState, (state) => state.selectedCount);
export const selectPhysicalCountsPage = createSelector(selectPhysicalCountsState, (state) => state.page);
export const selectPhysicalCountsPageSize = createSelector(selectPhysicalCountsState, (state) => state.pageSize);
export const selectPhysicalCountsTotalCount = createSelector(selectPhysicalCountsState, (state) => state.totalCount);
export const selectPhysicalCountsTotalPages = createSelector(selectPhysicalCountsState, (state) => state.totalPages);
export const selectPhysicalCountsLoading = createSelector(selectPhysicalCountsState, (state) => state.loading);
export const selectPhysicalCountDetailLoading = createSelector(selectPhysicalCountsState, (state) => state.detailLoading);
export const selectPhysicalCountsSaving = createSelector(selectPhysicalCountsState, (state) => state.saving);
export const selectPhysicalCountsError = createSelector(selectPhysicalCountsState, (state) => state.error);
export const selectPhysicalCountsSearch = createSelector(selectPhysicalCountsState, (state) => state.search);
export const selectPhysicalCountsWarehouseFilter = createSelector(selectPhysicalCountsState, (state) => state.warehouseId);
export const selectPhysicalCountsStatusFilter = createSelector(selectPhysicalCountsState, (state) => state.status);
export const selectPhysicalCountWarehouseOptions = createSelector(selectPhysicalCountsState, (state) => state.warehouseOptions);
export const selectPhysicalCountsLookupsLoading = createSelector(selectPhysicalCountsState, (state) => state.lookupsLoading);
