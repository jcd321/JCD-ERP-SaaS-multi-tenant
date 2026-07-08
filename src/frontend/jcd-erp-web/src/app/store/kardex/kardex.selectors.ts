import { createFeatureSelector, createSelector } from '@ngrx/store';

import { KardexState } from './kardex.state';

export const selectKardexState = createFeatureSelector<KardexState>('kardex');

export const selectAllKardexEntries = createSelector(selectKardexState, (state) => state.items);
export const selectKardexPage = createSelector(selectKardexState, (state) => state.page);
export const selectKardexPageSize = createSelector(selectKardexState, (state) => state.pageSize);
export const selectKardexTotalCount = createSelector(selectKardexState, (state) => state.totalCount);
export const selectKardexTotalPages = createSelector(selectKardexState, (state) => state.totalPages);
export const selectKardexProductFilter = createSelector(selectKardexState, (state) => state.productId);
export const selectKardexWarehouseFilter = createSelector(selectKardexState, (state) => state.warehouseId);
export const selectKardexFromDateFilter = createSelector(selectKardexState, (state) => state.fromDate);
export const selectKardexToDateFilter = createSelector(selectKardexState, (state) => state.toDate);
export const selectKardexProductOptions = createSelector(selectKardexState, (state) => state.productOptions);
export const selectKardexWarehouseOptions = createSelector(selectKardexState, (state) => state.warehouseOptions);
export const selectKardexLookupsLoading = createSelector(selectKardexState, (state) => state.lookupsLoading);
export const selectKardexLoading = createSelector(selectKardexState, (state) => state.loading);
export const selectKardexError = createSelector(selectKardexState, (state) => state.error);
