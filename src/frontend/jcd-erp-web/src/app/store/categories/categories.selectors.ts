import { createFeatureSelector, createSelector } from '@ngrx/store';

import { CategoriesState } from './categories.state';

export const selectCategoriesState = createFeatureSelector<CategoriesState>('categories');

export const selectAllCategories = createSelector(selectCategoriesState, (s) => s.items);
export const selectCategoryParentOptions = createSelector(selectCategoriesState, (s) => s.parentOptions);
export const selectCategoriesPage = createSelector(selectCategoriesState, (s) => s.page);
export const selectCategoriesPageSize = createSelector(selectCategoriesState, (s) => s.pageSize);
export const selectCategoriesTotalCount = createSelector(selectCategoriesState, (s) => s.totalCount);
export const selectCategoriesTotalPages = createSelector(selectCategoriesState, (s) => s.totalPages);
export const selectCategoriesSearch = createSelector(selectCategoriesState, (s) => s.search);
export const selectCategoriesLoading = createSelector(selectCategoriesState, (s) => s.loading);
export const selectCategoriesSaving = createSelector(selectCategoriesState, (s) => s.saving);
export const selectCategoriesError = createSelector(selectCategoriesState, (s) => s.error);
