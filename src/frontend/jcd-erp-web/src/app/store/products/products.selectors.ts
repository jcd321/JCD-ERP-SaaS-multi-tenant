import { createFeatureSelector, createSelector } from '@ngrx/store';

import { ProductsState } from './products.state';

export const selectProductsState = createFeatureSelector<ProductsState>('products');

export const selectAllProducts = createSelector(selectProductsState, (state) => state.items);

export const selectProductLookups = createSelector(selectProductsState, (state) => state.lookups);

export const selectProductsPage = createSelector(selectProductsState, (state) => state.page);

export const selectProductsPageSize = createSelector(selectProductsState, (state) => state.pageSize);

export const selectProductsTotalCount = createSelector(selectProductsState, (state) => state.totalCount);

export const selectProductsTotalPages = createSelector(selectProductsState, (state) => state.totalPages);

export const selectProductsSearch = createSelector(selectProductsState, (state) => state.search);

export const selectProductsLoading = createSelector(selectProductsState, (state) => state.loading);

export const selectProductsSaving = createSelector(selectProductsState, (state) => state.saving);

export const selectProductsError = createSelector(selectProductsState, (state) => state.error);
