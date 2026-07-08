import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { WarehousesQueryParams, CreateWarehouseRequest, UpdateWarehouseRequest } from '../../features/warehouses/warehouses.models';
import { WarehousesActions } from './warehouses.actions';
import {
  selectAllWarehouses,
  selectWarehousesError,
  selectWarehousesLoading,
  selectWarehousesPage,
  selectWarehousesPageSize,
  selectWarehousesSaving,
  selectWarehousesSearch,
  selectWarehousesTotalCount,
  selectWarehousesTotalPages,
} from './warehouses.selectors';

@Injectable({ providedIn: 'root' })
export class WarehousesFacade {
  private readonly store = inject(Store);

  readonly warehouses = toSignal(this.store.select(selectAllWarehouses), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectWarehousesPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectWarehousesPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectWarehousesTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectWarehousesTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectWarehousesSearch), { initialValue: '' });
  readonly loading = toSignal(this.store.select(selectWarehousesLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectWarehousesSaving), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectWarehousesError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadWarehouses(params?: WarehousesQueryParams): void {
    this.store.dispatch(WarehousesActions.loadWarehouses({ params }));
  }

  createWarehouse(request: CreateWarehouseRequest): void {
    this.store.dispatch(WarehousesActions.createWarehouse({ request }));
  }

  updateWarehouse(warehouseId: string, request: UpdateWarehouseRequest): void {
    this.store.dispatch(WarehousesActions.updateWarehouse({ warehouseId, request }));
  }

  deleteWarehouse(warehouseId: string): void {
    this.store.dispatch(WarehousesActions.deleteWarehouse({ warehouseId }));
  }
}
