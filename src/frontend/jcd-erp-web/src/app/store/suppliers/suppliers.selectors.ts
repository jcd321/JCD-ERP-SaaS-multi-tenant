import { createFeatureSelector, createSelector } from '@ngrx/store';

import { SuppliersState } from './suppliers.state';

export const selectSuppliersState = createFeatureSelector<SuppliersState>('suppliers');

export const selectAllSuppliers = createSelector(selectSuppliersState, (state) => state.items);

export const selectSuppliersPage = createSelector(selectSuppliersState, (state) => state.page);

export const selectSuppliersPageSize = createSelector(selectSuppliersState, (state) => state.pageSize);

export const selectSuppliersTotalCount = createSelector(selectSuppliersState, (state) => state.totalCount);

export const selectSuppliersTotalPages = createSelector(selectSuppliersState, (state) => state.totalPages);

export const selectSuppliersSearch = createSelector(selectSuppliersState, (state) => state.search);

export const selectSuppliersLoading = createSelector(selectSuppliersState, (state) => state.loading);

export const selectSuppliersSaving = createSelector(selectSuppliersState, (state) => state.saving);

export const selectSuppliersError = createSelector(selectSuppliersState, (state) => state.error);
