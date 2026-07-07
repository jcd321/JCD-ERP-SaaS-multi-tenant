import { createReducer, on } from '@ngrx/store';

import { BrandsActions } from './brands.actions';
import { initialBrandsState } from './brands.state';

export const brandsReducer = createReducer(
  initialBrandsState,

  on(BrandsActions.loadBrands, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(BrandsActions.loadBrandsSuccess, (state, { response, search }) => ({
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

  on(BrandsActions.loadBrandsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(BrandsActions.createBrand, BrandsActions.updateBrand, BrandsActions.deleteBrand, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(
    BrandsActions.createBrandSuccess,
    BrandsActions.updateBrandSuccess,
    BrandsActions.deleteBrandSuccess,
    (state) => ({
      ...state,
      saving: false,
      error: null,
    }),
  ),

  on(
    BrandsActions.createBrandFailure,
    BrandsActions.updateBrandFailure,
    BrandsActions.deleteBrandFailure,
    (state, { error }) => ({
      ...state,
      saving: false,
      error,
    }),
  ),
);
