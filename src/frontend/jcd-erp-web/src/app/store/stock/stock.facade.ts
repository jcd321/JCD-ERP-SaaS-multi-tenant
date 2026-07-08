import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import {
  CreateStockLevelRequest,
  StockQueryParams,
  UpdateStockLevelRequest,
} from '../../features/stock/stock.models';
import { StockActions } from './stock.actions';
import {
  selectAllStockLevels,
  selectStockBelowMinimumOnly,
  selectStockError,
  selectStockLoading,
  selectStockLookupsLoading,
  selectStockPage,
  selectStockPageSize,
  selectStockProductFilter,
  selectStockProductOptions,
  selectStockSaving,
  selectStockSearch,
  selectStockTotalCount,
  selectStockTotalPages,
  selectStockWarehouseFilter,
  selectStockWarehouseOptions,
} from './stock.selectors';

@Injectable({ providedIn: 'root' })
export class StockFacade {
  private readonly store = inject(Store);

  readonly stockLevels = toSignal(this.store.select(selectAllStockLevels), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectStockPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectStockPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectStockTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectStockTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectStockSearch), { initialValue: '' });
  readonly warehouseFilter = toSignal(this.store.select(selectStockWarehouseFilter), { initialValue: null });
  readonly productFilter = toSignal(this.store.select(selectStockProductFilter), { initialValue: null });
  readonly belowMinimumOnly = toSignal(this.store.select(selectStockBelowMinimumOnly), { initialValue: false });
  readonly productOptions = toSignal(this.store.select(selectStockProductOptions), { initialValue: [] });
  readonly warehouseOptions = toSignal(this.store.select(selectStockWarehouseOptions), { initialValue: [] });
  readonly lookupsLoading = toSignal(this.store.select(selectStockLookupsLoading), { initialValue: false });
  readonly loading = toSignal(this.store.select(selectStockLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectStockSaving), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectStockError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadStockLevels(params?: StockQueryParams): void {
    this.store.dispatch(StockActions.loadStockLevels({ params }));
  }

  loadLookups(): void {
    this.store.dispatch(StockActions.loadStockLookups());
  }

  createStockLevel(request: CreateStockLevelRequest): void {
    this.store.dispatch(StockActions.createStockLevel({ request }));
  }

  updateStockLevel(stockLevelId: string, request: UpdateStockLevelRequest): void {
    this.store.dispatch(StockActions.updateStockLevel({ stockLevelId, request }));
  }

  deleteStockLevel(stockLevelId: string): void {
    this.store.dispatch(StockActions.deleteStockLevel({ stockLevelId }));
  }
}
