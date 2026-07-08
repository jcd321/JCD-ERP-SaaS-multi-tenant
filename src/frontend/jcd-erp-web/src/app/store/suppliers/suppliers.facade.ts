import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import {
  CreateSupplierRequest,
  SuppliersQueryParams,
  UpdateSupplierRequest,
} from '../../features/suppliers/suppliers.models';
import { SuppliersActions } from './suppliers.actions';
import {
  selectAllSuppliers,
  selectSuppliersError,
  selectSuppliersLoading,
  selectSuppliersPage,
  selectSuppliersPageSize,
  selectSuppliersSaving,
  selectSuppliersSearch,
  selectSuppliersTotalCount,
  selectSuppliersTotalPages,
} from './suppliers.selectors';

@Injectable({ providedIn: 'root' })
export class SuppliersFacade {
  private readonly store = inject(Store);

  readonly suppliers = toSignal(this.store.select(selectAllSuppliers), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectSuppliersPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectSuppliersPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectSuppliersTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectSuppliersTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectSuppliersSearch), { initialValue: '' });
  readonly loading = toSignal(this.store.select(selectSuppliersLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectSuppliersSaving), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectSuppliersError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadSuppliers(params?: SuppliersQueryParams): void {
    this.store.dispatch(SuppliersActions.loadSuppliers({ params }));
  }

  createSupplier(request: CreateSupplierRequest): void {
    this.store.dispatch(SuppliersActions.createSupplier({ request }));
  }

  updateSupplier(supplierId: string, request: UpdateSupplierRequest): void {
    this.store.dispatch(SuppliersActions.updateSupplier({ supplierId, request }));
  }

  deleteSupplier(supplierId: string): void {
    this.store.dispatch(SuppliersActions.deleteSupplier({ supplierId }));
  }
}
