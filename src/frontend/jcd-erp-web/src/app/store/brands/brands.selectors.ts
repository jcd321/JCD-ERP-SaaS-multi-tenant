import { createFeatureSelector, createSelector } from '@ngrx/store';

import { BrandsState } from './brands.state';

export const selectBrandsState = createFeatureSelector<BrandsState>('brands');

export const selectAllBrands = createSelector(selectBrandsState, (state) => state.items);

export const selectBrandsPage = createSelector(selectBrandsState, (state) => state.page);

export const selectBrandsPageSize = createSelector(selectBrandsState, (state) => state.pageSize);

export const selectBrandsTotalCount = createSelector(selectBrandsState, (state) => state.totalCount);

export const selectBrandsTotalPages = createSelector(selectBrandsState, (state) => state.totalPages);

export const selectBrandsSearch = createSelector(selectBrandsState, (state) => state.search);

export const selectBrandsLoading = createSelector(selectBrandsState, (state) => state.loading);

export const selectBrandsSaving = createSelector(selectBrandsState, (state) => state.saving);

export const selectBrandsError = createSelector(selectBrandsState, (state) => state.error);
