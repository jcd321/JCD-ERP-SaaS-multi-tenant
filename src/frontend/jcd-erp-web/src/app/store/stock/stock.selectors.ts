import { createFeatureSelector, createSelector } from '@ngrx/store';

import { StockState } from './stock.state';

export const selectStockState = createFeatureSelector<StockState>('stock');

export const selectAllStockLevels = createSelector(selectStockState, (state) => state.items);

export const selectStockPage = createSelector(selectStockState, (state) => state.page);

export const selectStockPageSize = createSelector(selectStockState, (state) => state.pageSize);

export const selectStockTotalCount = createSelector(selectStockState, (state) => state.totalCount);

export const selectStockTotalPages = createSelector(selectStockState, (state) => state.totalPages);

export const selectStockSearch = createSelector(selectStockState, (state) => state.search);

export const selectStockWarehouseFilter = createSelector(selectStockState, (state) => state.warehouseId);

export const selectStockProductFilter = createSelector(selectStockState, (state) => state.productId);

export const selectStockBelowMinimumOnly = createSelector(selectStockState, (state) => state.belowMinimumOnly);

export const selectStockProductOptions = createSelector(selectStockState, (state) => state.productOptions);

export const selectStockWarehouseOptions = createSelector(selectStockState, (state) => state.warehouseOptions);

export const selectStockLookupsLoading = createSelector(selectStockState, (state) => state.lookupsLoading);

export const selectStockLoading = createSelector(selectStockState, (state) => state.loading);

export const selectStockSaving = createSelector(selectStockState, (state) => state.saving);

export const selectStockError = createSelector(selectStockState, (state) => state.error);
