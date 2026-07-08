import { createReducer, on } from '@ngrx/store';

import { SuppliersActions } from './suppliers.actions';
import { initialSuppliersState } from './suppliers.state';

export const suppliersReducer = createReducer(
  initialSuppliersState,

  on(SuppliersActions.loadSuppliers, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(SuppliersActions.loadSuppliersSuccess, (state, { response, search }) => ({
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

  on(SuppliersActions.loadSuppliersFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(SuppliersActions.createSupplier, SuppliersActions.updateSupplier, SuppliersActions.deleteSupplier, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(
    SuppliersActions.createSupplierSuccess,
    SuppliersActions.updateSupplierSuccess,
    SuppliersActions.deleteSupplierSuccess,
    (state) => ({
      ...state,
      saving: false,
      error: null,
    }),
  ),

  on(
    SuppliersActions.createSupplierFailure,
    SuppliersActions.updateSupplierFailure,
    SuppliersActions.deleteSupplierFailure,
    (state, { error }) => ({
      ...state,
      saving: false,
      error,
    }),
  ),
);
