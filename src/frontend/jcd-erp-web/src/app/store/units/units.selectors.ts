import { createFeatureSelector, createSelector } from '@ngrx/store';

import { UnitsState } from './units.state';

export const selectUnitsState = createFeatureSelector<UnitsState>('units');

export const selectAllUnits = createSelector(selectUnitsState, (state) => state.items);

export const selectUnitsPage = createSelector(selectUnitsState, (state) => state.page);

export const selectUnitsPageSize = createSelector(selectUnitsState, (state) => state.pageSize);

export const selectUnitsTotalCount = createSelector(selectUnitsState, (state) => state.totalCount);

export const selectUnitsTotalPages = createSelector(selectUnitsState, (state) => state.totalPages);

export const selectUnitsSearch = createSelector(selectUnitsState, (state) => state.search);

export const selectUnitsLoading = createSelector(selectUnitsState, (state) => state.loading);

export const selectUnitsSaving = createSelector(selectUnitsState, (state) => state.saving);

export const selectUnitsError = createSelector(selectUnitsState, (state) => state.error);
