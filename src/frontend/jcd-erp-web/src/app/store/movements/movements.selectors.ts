import { createFeatureSelector, createSelector } from '@ngrx/store';

import { MovementsState } from './movements.state';

export const selectMovementsState = createFeatureSelector<MovementsState>('movements');

export const selectAllMovements = createSelector(selectMovementsState, (state) => state.items);
export const selectMovementsPage = createSelector(selectMovementsState, (state) => state.page);
export const selectMovementsPageSize = createSelector(selectMovementsState, (state) => state.pageSize);
export const selectMovementsTotalCount = createSelector(selectMovementsState, (state) => state.totalCount);
export const selectMovementsTotalPages = createSelector(selectMovementsState, (state) => state.totalPages);
export const selectMovementsSearch = createSelector(selectMovementsState, (state) => state.search);
export const selectMovementsWarehouseFilter = createSelector(selectMovementsState, (state) => state.warehouseId);
export const selectMovementsProductFilter = createSelector(selectMovementsState, (state) => state.productId);
export const selectMovementsTypeFilter = createSelector(selectMovementsState, (state) => state.movementType);
export const selectMovementProductOptions = createSelector(selectMovementsState, (state) => state.productOptions);
export const selectMovementWarehouseOptions = createSelector(selectMovementsState, (state) => state.warehouseOptions);
export const selectMovementsLookupsLoading = createSelector(selectMovementsState, (state) => state.lookupsLoading);
export const selectMovementsLoading = createSelector(selectMovementsState, (state) => state.loading);
export const selectMovementsSaving = createSelector(selectMovementsState, (state) => state.saving);
export const selectMovementsError = createSelector(selectMovementsState, (state) => state.error);
