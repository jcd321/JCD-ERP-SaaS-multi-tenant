import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import {
  AdjustmentsQueryParams,
  CreateInventoryAdjustmentRequest,
} from '../../features/adjustments/adjustments.models';
import { AdjustmentsActions } from './adjustments.actions';
import {
  selectAdjustmentProductOptions,
  selectAdjustmentStockLevels,
  selectAdjustmentWarehouseOptions,
  selectAdjustmentsError,
  selectAdjustmentsLoading,
  selectAdjustmentsLookupsLoading,
  selectAdjustmentsPage,
  selectAdjustmentsPageSize,
  selectAdjustmentsSaving,
  selectAdjustmentsSearch,
  selectAdjustmentsTotalCount,
  selectAdjustmentsTotalPages,
  selectAdjustmentsWarehouseFilter,
  selectAllAdjustments,
} from './adjustments.selectors';

@Injectable({ providedIn: 'root' })
export class AdjustmentsFacade {
  private readonly store = inject(Store);

  readonly adjustments = toSignal(this.store.select(selectAllAdjustments), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectAdjustmentsPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectAdjustmentsPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectAdjustmentsTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectAdjustmentsTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectAdjustmentsSearch), { initialValue: '' });
  readonly warehouseFilter = toSignal(this.store.select(selectAdjustmentsWarehouseFilter), { initialValue: null });
  readonly productOptions = toSignal(this.store.select(selectAdjustmentProductOptions), { initialValue: [] });
  readonly warehouseOptions = toSignal(this.store.select(selectAdjustmentWarehouseOptions), { initialValue: [] });
  readonly stockLevels = toSignal(this.store.select(selectAdjustmentStockLevels), { initialValue: [] });
  readonly lookupsLoading = toSignal(this.store.select(selectAdjustmentsLookupsLoading), { initialValue: false });
  readonly loading = toSignal(this.store.select(selectAdjustmentsLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectAdjustmentsSaving), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectAdjustmentsError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadAdjustments(params?: AdjustmentsQueryParams): void {
    this.store.dispatch(AdjustmentsActions.loadAdjustments({ params }));
  }

  loadLookups(): void {
    this.store.dispatch(AdjustmentsActions.loadAdjustmentLookups());
  }

  createAdjustment(request: CreateInventoryAdjustmentRequest): void {
    this.store.dispatch(AdjustmentsActions.createAdjustment({ request }));
  }

  clearError(): void {
    this.store.dispatch(AdjustmentsActions.clearError());
  }
}
