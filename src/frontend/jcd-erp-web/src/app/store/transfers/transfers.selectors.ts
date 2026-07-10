import { createFeatureSelector, createSelector } from '@ngrx/store';

import { TransfersState } from './transfers.state';

export const selectTransfersState = createFeatureSelector<TransfersState>('transfers');

export const selectAllTransfers = createSelector(selectTransfersState, (state) => state.items);
export const selectTransfersPage = createSelector(selectTransfersState, (state) => state.page);
export const selectTransfersPageSize = createSelector(selectTransfersState, (state) => state.pageSize);
export const selectTransfersTotalCount = createSelector(selectTransfersState, (state) => state.totalCount);
export const selectTransfersTotalPages = createSelector(selectTransfersState, (state) => state.totalPages);
export const selectTransfersSearch = createSelector(selectTransfersState, (state) => state.search);
export const selectTransfersSourceFilter = createSelector(selectTransfersState, (state) => state.sourceWarehouseId);
export const selectTransfersDestinationFilter = createSelector(selectTransfersState, (state) => state.destinationWarehouseId);
export const selectTransferProductOptions = createSelector(selectTransfersState, (state) => state.productOptions);
export const selectTransferWarehouseOptions = createSelector(selectTransfersState, (state) => state.warehouseOptions);
export const selectTransferStockLevels = createSelector(selectTransfersState, (state) => state.stockLevels);
export const selectTransfersLookupsLoading = createSelector(selectTransfersState, (state) => state.lookupsLoading);
export const selectTransfersLoading = createSelector(selectTransfersState, (state) => state.loading);
export const selectTransfersSaving = createSelector(selectTransfersState, (state) => state.saving);
export const selectTransfersError = createSelector(selectTransfersState, (state) => state.error);
