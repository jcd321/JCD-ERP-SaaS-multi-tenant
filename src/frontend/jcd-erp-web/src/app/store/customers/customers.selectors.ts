import { createFeatureSelector, createSelector } from '@ngrx/store';

import { CustomersState } from './customers.state';

export const selectCustomersState = createFeatureSelector<CustomersState>('customers');

export const selectAllCustomers = createSelector(selectCustomersState, (state) => state.items);

export const selectCustomersPage = createSelector(selectCustomersState, (state) => state.page);

export const selectCustomersPageSize = createSelector(selectCustomersState, (state) => state.pageSize);

export const selectCustomersTotalCount = createSelector(selectCustomersState, (state) => state.totalCount);

export const selectCustomersTotalPages = createSelector(selectCustomersState, (state) => state.totalPages);

export const selectCustomersSearch = createSelector(selectCustomersState, (state) => state.search);

export const selectCustomersLoading = createSelector(selectCustomersState, (state) => state.loading);

export const selectCustomersSaving = createSelector(selectCustomersState, (state) => state.saving);

export const selectCustomersError = createSelector(selectCustomersState, (state) => state.error);
