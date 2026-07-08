import { createFeatureSelector, createSelector } from '@ngrx/store';

import { LocationsState } from './locations.state';

export const selectLocationsState = createFeatureSelector<LocationsState>('locations');

export const selectAllLocations = createSelector(selectLocationsState, (s) => s.items);
export const selectLocationsWarehouseId = createSelector(selectLocationsState, (s) => s.warehouseId);
export const selectLocationParentOptions = createSelector(selectLocationsState, (s) => s.parentOptions);
export const selectLocationsPage = createSelector(selectLocationsState, (s) => s.page);
export const selectLocationsPageSize = createSelector(selectLocationsState, (s) => s.pageSize);
export const selectLocationsTotalCount = createSelector(selectLocationsState, (s) => s.totalCount);
export const selectLocationsTotalPages = createSelector(selectLocationsState, (s) => s.totalPages);
export const selectLocationsSearch = createSelector(selectLocationsState, (s) => s.search);
export const selectLocationsLoading = createSelector(selectLocationsState, (s) => s.loading);
export const selectLocationsSaving = createSelector(selectLocationsState, (s) => s.saving);
export const selectLocationsError = createSelector(selectLocationsState, (s) => s.error);
