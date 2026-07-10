import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { CreateInventoryTransferRequest, TransfersQueryParams } from '../../features/transfers/transfers.models';
import { TransfersActions } from './transfers.actions';
import {
  selectAllTransfers,
  selectTransferProductOptions,
  selectTransferStockLevels,
  selectTransferWarehouseOptions,
  selectTransfersDestinationFilter,
  selectTransfersError,
  selectTransfersLoading,
  selectTransfersLookupsLoading,
  selectTransfersPage,
  selectTransfersPageSize,
  selectTransfersSaving,
  selectTransfersSearch,
  selectTransfersSourceFilter,
  selectTransfersTotalCount,
  selectTransfersTotalPages,
} from './transfers.selectors';

@Injectable({ providedIn: 'root' })
export class TransfersFacade {
  private readonly store = inject(Store);

  readonly transfers = toSignal(this.store.select(selectAllTransfers), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectTransfersPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectTransfersPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectTransfersTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectTransfersTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectTransfersSearch), { initialValue: '' });
  readonly sourceFilter = toSignal(this.store.select(selectTransfersSourceFilter), { initialValue: null });
  readonly destinationFilter = toSignal(this.store.select(selectTransfersDestinationFilter), { initialValue: null });
  readonly productOptions = toSignal(this.store.select(selectTransferProductOptions), { initialValue: [] });
  readonly warehouseOptions = toSignal(this.store.select(selectTransferWarehouseOptions), { initialValue: [] });
  readonly stockLevels = toSignal(this.store.select(selectTransferStockLevels), { initialValue: [] });
  readonly lookupsLoading = toSignal(this.store.select(selectTransfersLookupsLoading), { initialValue: false });
  readonly loading = toSignal(this.store.select(selectTransfersLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectTransfersSaving), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectTransfersError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadTransfers(params?: TransfersQueryParams): void {
    this.store.dispatch(TransfersActions.loadTransfers({ params }));
  }

  loadLookups(): void {
    this.store.dispatch(TransfersActions.loadTransferLookups());
  }

  createTransfer(request: CreateInventoryTransferRequest): void {
    this.store.dispatch(TransfersActions.createTransfer({ request }));
  }

  clearError(): void {
    this.store.dispatch(TransfersActions.clearError());
  }
}
