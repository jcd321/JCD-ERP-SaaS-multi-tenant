import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { WarehousesService } from '../../features/warehouses/warehouses.service';
import { WarehousesActions } from './warehouses.actions';
import { selectWarehousesPage, selectWarehousesPageSize, selectWarehousesSearch } from './warehouses.selectors';

@Injectable()
export class WarehousesEffects {
  private readonly actions$ = inject(Actions);
  private readonly warehousesService = inject(WarehousesService);
  private readonly store = inject(Store);

  readonly loadWarehouses$ = createEffect(() =>
    this.actions$.pipe(
      ofType(WarehousesActions.loadWarehouses),
      withLatestFrom(
        this.store.select(selectWarehousesPage),
        this.store.select(selectWarehousesPageSize),
        this.store.select(selectWarehousesSearch),
      ),
      exhaustMap(([{ params }, page, pageSize, search]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          isActive: params?.isActive,
        };

        return this.warehousesService.getWarehouses(query).pipe(
          map((response) =>
            WarehousesActions.loadWarehousesSuccess({
              response,
              search: query.search ?? '',
            }),
          ),
          catchError((error) =>
            of(WarehousesActions.loadWarehousesFailure({
              error: extractPlatformErrorCode(error, 'Warehouses.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly createWarehouse$ = createEffect(() =>
    this.actions$.pipe(
      ofType(WarehousesActions.createWarehouse),
      exhaustMap(({ request }) =>
        this.warehousesService.createWarehouse(request).pipe(
          map(() => WarehousesActions.createWarehouseSuccess()),
          catchError((error) =>
            of(WarehousesActions.createWarehouseFailure({
              error: extractPlatformErrorCode(error, 'Warehouses.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateWarehouse$ = createEffect(() =>
    this.actions$.pipe(
      ofType(WarehousesActions.updateWarehouse),
      exhaustMap(({ warehouseId, request }) =>
        this.warehousesService.updateWarehouse(warehouseId, request).pipe(
          map(() => WarehousesActions.updateWarehouseSuccess()),
          catchError((error) =>
            of(WarehousesActions.updateWarehouseFailure({
              error: extractPlatformErrorCode(error, 'Warehouses.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly deleteWarehouse$ = createEffect(() =>
    this.actions$.pipe(
      ofType(WarehousesActions.deleteWarehouse),
      exhaustMap(({ warehouseId }) =>
        this.warehousesService.deleteWarehouse(warehouseId).pipe(
          map(() => WarehousesActions.deleteWarehouseSuccess()),
          catchError((error) =>
            of(WarehousesActions.deleteWarehouseFailure({
              error: extractPlatformErrorCode(error, 'Warehouses.DeleteFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(WarehousesActions.createWarehouseSuccess, WarehousesActions.updateWarehouseSuccess, WarehousesActions.deleteWarehouseSuccess),
      switchMap(() => [WarehousesActions.loadWarehouses({})]),
    ),
  );
}
