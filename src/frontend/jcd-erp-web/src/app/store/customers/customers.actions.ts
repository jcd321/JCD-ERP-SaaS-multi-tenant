import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  CreateCustomerRequest,
  CustomersQueryParams,
  PaginatedCustomersResponse,
  UpdateCustomerRequest,
} from '../../features/customers/customers.models';

export const CustomersActions = createActionGroup({
  source: 'Customers',
  events: {
    'Load Customers': props<{ params?: CustomersQueryParams }>(),
    'Load Customers Success': props<{ response: PaginatedCustomersResponse; search: string }>(),
    'Load Customers Failure': props<{ error: string }>(),

    'Create Customer': props<{ request: CreateCustomerRequest }>(),
    'Create Customer Success': emptyProps(),
    'Create Customer Failure': props<{ error: string }>(),

    'Update Customer': props<{ customerId: string; request: UpdateCustomerRequest }>(),
    'Update Customer Success': emptyProps(),
    'Update Customer Failure': props<{ error: string }>(),

    'Delete Customer': props<{ customerId: string }>(),
    'Delete Customer Success': emptyProps(),
    'Delete Customer Failure': props<{ error: string }>(),
  },
});
