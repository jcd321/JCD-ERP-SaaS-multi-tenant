import { inject, Injectable } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { CreateUnitRequest, UnitsQueryParams, UpdateUnitRequest } from '../../features/units/units.models';
import { UnitsActions } from './units.actions';
import {
  selectAllUnits,
  selectUnitsError,
  selectUnitsLoading,
  selectUnitsPage,
  selectUnitsPageSize,
  selectUnitsSaving,
  selectUnitsSearch,
  selectUnitsTotalCount,
  selectUnitsTotalPages,
} from './units.selectors';

@Injectable({ providedIn: 'root' })
export class UnitsFacade {
  private readonly store = inject(Store);

  readonly units = toSignal(this.store.select(selectAllUnits), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectUnitsPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectUnitsPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectUnitsTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectUnitsTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectUnitsSearch), { initialValue: '' });
  readonly loading = toSignal(this.store.select(selectUnitsLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectUnitsSaving), { initialValue: false });
  readonly error = toSignal(this.store.select(selectUnitsError), { initialValue: null });

  loadUnits(params?: UnitsQueryParams): void {
    this.store.dispatch(UnitsActions.loadUnits({ params }));
  }

  createUnit(request: CreateUnitRequest): void {
    this.store.dispatch(UnitsActions.createUnit({ request }));
  }

  updateUnit(unitId: string, request: UpdateUnitRequest): void {
    this.store.dispatch(UnitsActions.updateUnit({ unitId, request }));
  }

  deleteUnit(unitId: string): void {
    this.store.dispatch(UnitsActions.deleteUnit({ unitId }));
  }
}
