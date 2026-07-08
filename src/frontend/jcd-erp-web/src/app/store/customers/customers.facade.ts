import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import {
  CreateCustomerRequest,
  CustomersQueryParams,
  UpdateCustomerRequest,
} from '../../features/customers/customers.models';
import { CustomersActions } from './customers.actions';
import {
  selectAllCustomers,
  selectCustomersError,
  selectCustomersLoading,
  selectCustomersPage,
  selectCustomersPageSize,
  selectCustomersSaving,
  selectCustomersSearch,
  selectCustomersTotalCount,
  selectCustomersTotalPages,
} from './customers.selectors';

@Injectable({ providedIn: 'root' })
export class CustomersFacade {
  private readonly store = inject(Store);

  readonly customers = toSignal(this.store.select(selectAllCustomers), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectCustomersPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectCustomersPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectCustomersTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectCustomersTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectCustomersSearch), { initialValue: '' });
  readonly loading = toSignal(this.store.select(selectCustomersLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectCustomersSaving), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectCustomersError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadCustomers(params?: CustomersQueryParams): void {
    this.store.dispatch(CustomersActions.loadCustomers({ params }));
  }

  createCustomer(request: CreateCustomerRequest): void {
    this.store.dispatch(CustomersActions.createCustomer({ request }));
  }

  updateCustomer(customerId: string, request: UpdateCustomerRequest): void {
    this.store.dispatch(CustomersActions.updateCustomer({ customerId, request }));
  }

  deleteCustomer(customerId: string): void {
    this.store.dispatch(CustomersActions.deleteCustomer({ customerId }));
  }
}
