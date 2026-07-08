import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { CreateInventoryMovementRequest, MovementsQueryParams } from '../../features/movements/movements.models';
import { MovementsActions } from './movements.actions';
import {
  selectAllMovements,
  selectMovementProductOptions,
  selectMovementWarehouseOptions,
  selectMovementsError,
  selectMovementsLoading,
  selectMovementsLookupsLoading,
  selectMovementsPage,
  selectMovementsPageSize,
  selectMovementsProductFilter,
  selectMovementsSaving,
  selectMovementsSearch,
  selectMovementsTotalCount,
  selectMovementsTotalPages,
  selectMovementsTypeFilter,
  selectMovementsWarehouseFilter,
} from './movements.selectors';

@Injectable({ providedIn: 'root' })
export class MovementsFacade {
  private readonly store = inject(Store);

  readonly movements = toSignal(this.store.select(selectAllMovements), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectMovementsPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectMovementsPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectMovementsTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectMovementsTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectMovementsSearch), { initialValue: '' });
  readonly warehouseFilter = toSignal(this.store.select(selectMovementsWarehouseFilter), { initialValue: null });
  readonly productFilter = toSignal(this.store.select(selectMovementsProductFilter), { initialValue: null });
  readonly movementTypeFilter = toSignal(this.store.select(selectMovementsTypeFilter), { initialValue: null });
  readonly productOptions = toSignal(this.store.select(selectMovementProductOptions), { initialValue: [] });
  readonly warehouseOptions = toSignal(this.store.select(selectMovementWarehouseOptions), { initialValue: [] });
  readonly lookupsLoading = toSignal(this.store.select(selectMovementsLookupsLoading), { initialValue: false });
  readonly loading = toSignal(this.store.select(selectMovementsLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectMovementsSaving), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectMovementsError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadMovements(params?: MovementsQueryParams): void {
    this.store.dispatch(MovementsActions.loadMovements({ params }));
  }

  loadLookups(): void {
    this.store.dispatch(MovementsActions.loadMovementLookups());
  }

  createMovement(request: CreateInventoryMovementRequest): void {
    this.store.dispatch(MovementsActions.createMovement({ request }));
  }
}
