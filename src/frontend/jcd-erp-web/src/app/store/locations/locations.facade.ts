import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import {
  LocationsQueryParams,
  CreateLocationRequest,
  UpdateLocationRequest,
} from '../../features/locations/locations.models';
import { LocationsActions } from './locations.actions';
import {
  selectAllLocations,
  selectLocationsError,
  selectLocationsLoading,
  selectLocationsPage,
  selectLocationsPageSize,
  selectLocationsSaving,
  selectLocationsSearch,
  selectLocationsTotalCount,
  selectLocationsTotalPages,
  selectLocationParentOptions,
} from './locations.selectors';

@Injectable({ providedIn: 'root' })
export class LocationsFacade {
  private readonly store = inject(Store);

  readonly locations = toSignal(this.store.select(selectAllLocations), { initialValue: [] });
  readonly parentOptions = toSignal(this.store.select(selectLocationParentOptions), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectLocationsPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectLocationsPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectLocationsTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectLocationsTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectLocationsSearch), { initialValue: '' });
  readonly loading = toSignal(this.store.select(selectLocationsLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectLocationsSaving), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectLocationsError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadLocations(params?: LocationsQueryParams): void {
    this.store.dispatch(LocationsActions.loadLocations({ params }));
  }

  loadParentOptions(warehouseId: string, excludeId?: string): void {
    this.store.dispatch(LocationsActions.loadParentOptions({ warehouseId, excludeId }));
  }

  createLocation(request: CreateLocationRequest): void {
    this.store.dispatch(LocationsActions.createLocation({ request }));
  }

  updateLocation(locationId: string, request: UpdateLocationRequest): void {
    this.store.dispatch(LocationsActions.updateLocation({ locationId, request }));
  }

  deleteLocation(locationId: string): void {
    this.store.dispatch(LocationsActions.deleteLocation({ locationId }));
  }
}
