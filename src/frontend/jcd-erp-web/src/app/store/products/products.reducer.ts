import { createReducer, on } from '@ngrx/store';

import { ProductsActions } from './products.actions';
import { initialProductsState } from './products.state';

export const productsReducer = createReducer(
  initialProductsState,

  on(ProductsActions.loadProducts, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(ProductsActions.loadProductsSuccess, (state, { response, search }) => ({
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

  on(ProductsActions.loadProductsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(ProductsActions.loadLookupsSuccess, (state, { lookups }) => ({
    ...state,
    lookups,
  })),

  on(ProductsActions.createProduct, ProductsActions.updateProduct, ProductsActions.deleteProduct, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(
    ProductsActions.createProductSuccess,
    ProductsActions.updateProductSuccess,
    ProductsActions.deleteProductSuccess,
    (state) => ({
      ...state,
      saving: false,
      error: null,
    }),
  ),

  on(
    ProductsActions.createProductFailure,
    ProductsActions.updateProductFailure,
    ProductsActions.deleteProductFailure,
    (state, { error }) => ({
      ...state,
      saving: false,
      error,
    }),
  ),
);
