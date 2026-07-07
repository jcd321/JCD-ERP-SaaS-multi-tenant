import { createReducer, on } from '@ngrx/store';

import { CategoriesActions } from './categories.actions';
import { initialCategoriesState } from './categories.state';

export const categoriesReducer = createReducer(
  initialCategoriesState,

  on(CategoriesActions.loadCategories, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(CategoriesActions.loadCategoriesSuccess, (state, { response, search }) => ({
    ...state,
    items: response.items,
    page: response.page,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    totalPages: response.totalPages,
    search,
    loading: false,
    error: null,
  })),

  on(CategoriesActions.loadCategoriesFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(CategoriesActions.loadParentOptionsSuccess, (state, { options }) => ({
    ...state,
    parentOptions: options,
  })),

  on(
    CategoriesActions.createCategory,
    CategoriesActions.updateCategory,
    CategoriesActions.deleteCategory,
    (state) => ({ ...state, saving: true, error: null }),
  ),

  on(
    CategoriesActions.createCategorySuccess,
    CategoriesActions.updateCategorySuccess,
    CategoriesActions.deleteCategorySuccess,
    (state) => ({ ...state, saving: false, error: null }),
  ),

  on(
    CategoriesActions.createCategoryFailure,
    CategoriesActions.updateCategoryFailure,
    CategoriesActions.deleteCategoryFailure,
    (state, { error }) => ({ ...state, saving: false, error }),
  ),
);
