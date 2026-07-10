import { inject, Injectable } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import {
  CreatePhysicalCountRequest,
  PhysicalCountLineUpdate,
  PhysicalCountsQueryParams,
} from '../../features/physical-counts/physical-counts.models';
import { PhysicalCountsActions } from './physical-counts.actions';
import {
  selectPhysicalCountDetailLoading,
  selectPhysicalCountWarehouseOptions,
  selectPhysicalCounts,
  selectPhysicalCountsError,
  selectPhysicalCountsLoading,
  selectPhysicalCountsLookupsLoading,
  selectPhysicalCountsPage,
  selectPhysicalCountsSaving,
  selectPhysicalCountsSearch,
  selectPhysicalCountsStatusFilter,
  selectPhysicalCountsTotalCount,
  selectPhysicalCountsTotalPages,
  selectPhysicalCountsWarehouseFilter,
  selectSelectedPhysicalCount,
} from './physical-counts.selectors';

@Injectable({ providedIn: 'root' })
export class PhysicalCountsFacade {
  private readonly store = inject(Store);

  readonly physicalCounts = toSignal(this.store.select(selectPhysicalCounts), { initialValue: [] });
  readonly selectedCount = toSignal(this.store.select(selectSelectedPhysicalCount), { initialValue: null });
  readonly page = toSignal(this.store.select(selectPhysicalCountsPage), { initialValue: 1 });
  readonly totalPages = toSignal(this.store.select(selectPhysicalCountsTotalPages), { initialValue: 0 });
  readonly totalCount = toSignal(this.store.select(selectPhysicalCountsTotalCount), { initialValue: 0 });
  readonly loading = toSignal(this.store.select(selectPhysicalCountsLoading), { initialValue: false });
  readonly detailLoading = toSignal(this.store.select(selectPhysicalCountDetailLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectPhysicalCountsSaving), { initialValue: false });
  readonly error = toSignal(this.store.select(selectPhysicalCountsError), { initialValue: null });
  readonly search = toSignal(this.store.select(selectPhysicalCountsSearch), { initialValue: '' });
  readonly warehouseFilter = toSignal(this.store.select(selectPhysicalCountsWarehouseFilter), { initialValue: null });
  readonly statusFilter = toSignal(this.store.select(selectPhysicalCountsStatusFilter), { initialValue: null });
  readonly warehouseOptions = toSignal(this.store.select(selectPhysicalCountWarehouseOptions), { initialValue: [] });
  readonly lookupsLoading = toSignal(this.store.select(selectPhysicalCountsLookupsLoading), { initialValue: false });

  loadPhysicalCounts(params?: PhysicalCountsQueryParams): void {
    this.store.dispatch(PhysicalCountsActions.loadPhysicalCounts({ params }));
  }

  loadPhysicalCount(id: string): void {
    this.store.dispatch(PhysicalCountsActions.loadPhysicalCount({ id }));
  }

  loadLookups(): void {
    this.store.dispatch(PhysicalCountsActions.loadPhysicalCountLookups());
  }

  createPhysicalCount(request: CreatePhysicalCountRequest): void {
    this.store.dispatch(PhysicalCountsActions.createPhysicalCount({ request }));
  }

  updateLines(countId: string, lines: PhysicalCountLineUpdate[]): void {
    this.store.dispatch(PhysicalCountsActions.updatePhysicalCountLines({ countId, lines }));
  }

  complete(countId: string, lines?: PhysicalCountLineUpdate[]): void {
    this.store.dispatch(PhysicalCountsActions.completePhysicalCount({ countId, lines }));
  }

  cancel(countId: string): void {
    this.store.dispatch(PhysicalCountsActions.cancelPhysicalCount({ countId }));
  }

  clearError(): void {
    this.store.dispatch(PhysicalCountsActions.clearError());
  }
}
