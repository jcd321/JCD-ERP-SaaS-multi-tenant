import { createFeatureSelector, createSelector } from '@ngrx/store';

import { AdjustmentsState } from './adjustments.state';

export const selectAdjustmentsState = createFeatureSelector<AdjustmentsState>('adjustments');

export const selectAllAdjustments = createSelector(selectAdjustmentsState, (state) => state.items);
export const selectAdjustmentsPage = createSelector(selectAdjustmentsState, (state) => state.page);
export const selectAdjustmentsPageSize = createSelector(selectAdjustmentsState, (state) => state.pageSize);
export const selectAdjustmentsTotalCount = createSelector(selectAdjustmentsState, (state) => state.totalCount);
export const selectAdjustmentsTotalPages = createSelector(selectAdjustmentsState, (state) => state.totalPages);
export const selectAdjustmentsSearch = createSelector(selectAdjustmentsState, (state) => state.search);
export const selectAdjustmentsWarehouseFilter = createSelector(selectAdjustmentsState, (state) => state.warehouseId);
export const selectAdjustmentProductOptions = createSelector(selectAdjustmentsState, (state) => state.productOptions);
export const selectAdjustmentWarehouseOptions = createSelector(selectAdjustmentsState, (state) => state.warehouseOptions);
export const selectAdjustmentStockLevels = createSelector(selectAdjustmentsState, (state) => state.stockLevels);
export const selectAdjustmentsLookupsLoading = createSelector(selectAdjustmentsState, (state) => state.lookupsLoading);
export const selectAdjustmentsLoading = createSelector(selectAdjustmentsState, (state) => state.loading);
export const selectAdjustmentsSaving = createSelector(selectAdjustmentsState, (state) => state.saving);
export const selectAdjustmentsError = createSelector(selectAdjustmentsState, (state) => state.error);
