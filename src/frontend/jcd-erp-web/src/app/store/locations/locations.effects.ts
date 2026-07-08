import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { LocationsService } from '../../features/locations/locations.service';
import { LocationsActions } from './locations.actions';
import {
  selectLocationsPage,
  selectLocationsPageSize,
  selectLocationsSearch,
  selectLocationsWarehouseId,
} from './locations.selectors';

@Injectable()
export class LocationsEffects {
  private readonly actions$ = inject(Actions);
  private readonly locationsService = inject(LocationsService);
  private readonly store = inject(Store);

  readonly loadLocations$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LocationsActions.loadLocations),
      withLatestFrom(
        this.store.select(selectLocationsWarehouseId),
        this.store.select(selectLocationsPage),
        this.store.select(selectLocationsPageSize),
        this.store.select(selectLocationsSearch),
      ),
      exhaustMap(([{ params }, warehouseId, page, pageSize, search]) => {
        const resolvedWarehouseId = params?.warehouseId ?? warehouseId;
        if (!resolvedWarehouseId) {
          return of(LocationsActions.loadLocationsFailure({ error: 'Location.WarehouseRequired' }));
        }

        const query = {
          warehouseId: resolvedWarehouseId,
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          isActive: params?.isActive,
        };

        return this.locationsService.getLocations(query).pipe(
          map((response) =>
            LocationsActions.loadLocationsSuccess({
              response,
              search: query.search ?? '',
              warehouseId: resolvedWarehouseId,
            }),
          ),
          catchError((error) =>
            of(LocationsActions.loadLocationsFailure({
              error: extractPlatformErrorCode(error, 'Locations.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly loadParentOptions$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LocationsActions.loadParentOptions),
      exhaustMap(({ warehouseId, excludeId }) =>
        this.locationsService.getParentOptions(warehouseId, excludeId).pipe(
          map((options) => LocationsActions.loadParentOptionsSuccess({ options })),
          catchError((error) =>
            of(LocationsActions.loadParentOptionsFailure({
              error: extractPlatformErrorCode(error, 'Locations.ParentOptionsLoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly createLocation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LocationsActions.createLocation),
      exhaustMap(({ request }) =>
        this.locationsService.createLocation(request).pipe(
          map(() => LocationsActions.createLocationSuccess()),
          catchError((error) =>
            of(LocationsActions.createLocationFailure({
              error: extractPlatformErrorCode(error, 'Locations.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateLocation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LocationsActions.updateLocation),
      exhaustMap(({ locationId, request }) =>
        this.locationsService.updateLocation(locationId, request).pipe(
          map(() => LocationsActions.updateLocationSuccess()),
          catchError((error) =>
            of(LocationsActions.updateLocationFailure({
              error: extractPlatformErrorCode(error, 'Locations.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly deleteLocation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LocationsActions.deleteLocation),
      exhaustMap(({ locationId }) =>
        this.locationsService.deleteLocation(locationId).pipe(
          map(() => LocationsActions.deleteLocationSuccess()),
          catchError((error) =>
            of(LocationsActions.deleteLocationFailure({
              error: extractPlatformErrorCode(error, 'Locations.DeleteFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        LocationsActions.createLocationSuccess,
        LocationsActions.updateLocationSuccess,
        LocationsActions.deleteLocationSuccess,
      ),
      withLatestFrom(this.store.select(selectLocationsWarehouseId)),
      switchMap(([, warehouseId]) => {
        if (!warehouseId) return [];
        return [
          LocationsActions.loadLocations({ params: { warehouseId } }),
          LocationsActions.loadParentOptions({ warehouseId }),
        ];
      }),
    ),
  );
}
