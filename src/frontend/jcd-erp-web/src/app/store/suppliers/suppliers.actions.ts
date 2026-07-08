import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  CreateSupplierRequest,
  SuppliersQueryParams,
  PaginatedSuppliersResponse,
  UpdateSupplierRequest,
} from '../../features/suppliers/suppliers.models';

export const SuppliersActions = createActionGroup({
  source: 'Suppliers',
  events: {
    'Load Suppliers': props<{ params?: SuppliersQueryParams }>(),
    'Load Suppliers Success': props<{ response: PaginatedSuppliersResponse; search: string }>(),
    'Load Suppliers Failure': props<{ error: string }>(),

    'Create Supplier': props<{ request: CreateSupplierRequest }>(),
    'Create Supplier Success': emptyProps(),
    'Create Supplier Failure': props<{ error: string }>(),

    'Update Supplier': props<{ supplierId: string; request: UpdateSupplierRequest }>(),
    'Update Supplier Success': emptyProps(),
    'Update Supplier Failure': props<{ error: string }>(),

    'Delete Supplier': props<{ supplierId: string }>(),
    'Delete Supplier Success': emptyProps(),
    'Delete Supplier Failure': props<{ error: string }>(),
  },
});
