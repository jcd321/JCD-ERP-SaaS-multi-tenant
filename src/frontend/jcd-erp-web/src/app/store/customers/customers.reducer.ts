import { createReducer, on } from '@ngrx/store';

import { CustomersActions } from './customers.actions';
import { initialCustomersState } from './customers.state';

export const customersReducer = createReducer(
  initialCustomersState,

  on(CustomersActions.loadCustomers, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(CustomersActions.loadCustomersSuccess, (state, { response, search }) => ({
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

  on(CustomersActions.loadCustomersFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(CustomersActions.createCustomer, CustomersActions.updateCustomer, CustomersActions.deleteCustomer, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(
    CustomersActions.createCustomerSuccess,
    CustomersActions.updateCustomerSuccess,
    CustomersActions.deleteCustomerSuccess,
    (state) => ({
      ...state,
      saving: false,
      error: null,
    }),
  ),

  on(
    CustomersActions.createCustomerFailure,
    CustomersActions.updateCustomerFailure,
    CustomersActions.deleteCustomerFailure,
    (state, { error }) => ({
      ...state,
      saving: false,
      error,
    }),
  ),
);
