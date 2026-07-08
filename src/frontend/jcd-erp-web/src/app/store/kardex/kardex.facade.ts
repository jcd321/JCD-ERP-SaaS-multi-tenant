import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { KardexQueryParams } from '../../features/kardex/kardex.models';
import { KardexActions } from './kardex.actions';
import {
  selectAllKardexEntries,
  selectKardexError,
  selectKardexFromDateFilter,
  selectKardexLoading,
  selectKardexLookupsLoading,
  selectKardexPage,
  selectKardexPageSize,
  selectKardexProductFilter,
  selectKardexProductOptions,
  selectKardexToDateFilter,
  selectKardexTotalCount,
  selectKardexTotalPages,
  selectKardexWarehouseFilter,
  selectKardexWarehouseOptions,
} from './kardex.selectors';

@Injectable({ providedIn: 'root' })
export class KardexFacade {
  private readonly store = inject(Store);

  readonly entries = toSignal(this.store.select(selectAllKardexEntries), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectKardexPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectKardexPageSize), { initialValue: 50 });
  readonly totalCount = toSignal(this.store.select(selectKardexTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectKardexTotalPages), { initialValue: 0 });
  readonly productFilter = toSignal(this.store.select(selectKardexProductFilter), { initialValue: null });
  readonly warehouseFilter = toSignal(this.store.select(selectKardexWarehouseFilter), { initialValue: null });
  readonly fromDateFilter = toSignal(this.store.select(selectKardexFromDateFilter), { initialValue: null });
  readonly toDateFilter = toSignal(this.store.select(selectKardexToDateFilter), { initialValue: null });
  readonly productOptions = toSignal(this.store.select(selectKardexProductOptions), { initialValue: [] });
  readonly warehouseOptions = toSignal(this.store.select(selectKardexWarehouseOptions), { initialValue: [] });
  readonly lookupsLoading = toSignal(this.store.select(selectKardexLookupsLoading), { initialValue: false });
  readonly loading = toSignal(this.store.select(selectKardexLoading), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectKardexError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadKardex(params?: KardexQueryParams): void {
    this.store.dispatch(KardexActions.loadKardex({ params }));
  }

  loadLookups(): void {
    this.store.dispatch(KardexActions.loadKardexLookups());
  }
}
